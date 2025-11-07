using StackExchange.Redis;
using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Common.RateLimiter;

namespace GlobalAuth.Infrastructure.Services
{
    public class CustomRateLimiter : ICustomRateLimiter
    {
        private readonly IDatabase _redis;

        private static readonly string _incrWithTtl = @"
            local current = redis.call('INCR', KEYS[1])
            if current == 1 then
              redis.call('EXPIRE', KEYS[1], ARGV[1])
            end
            local ttl = redis.call('TTL', KEYS[1])
            return { current, ttl }
            ";

        public CustomRateLimiter(IConnectionMultiplexer connection)
        {
            _redis = connection.GetDatabase();
        }

        private static string BuildKey(string key, RateLimitRule rule)
            => $"rl:{rule.Policy}:{key}";

        public async Task<RateLimitResult> IncrementAndCheckAsync(string key, RateLimitRule rule)
        {
            if (rule.Limit <= 0 || rule.WindowSeconds <= 0)
                throw new ArgumentException("Invalid rate limit rule.");

            RedisKey redisKey = (RedisKey)BuildKey(key, rule);
            RedisValue window = (RedisValue)rule.WindowSeconds;

            var res = (RedisResult[]?)await _redis.ScriptEvaluateAsync(_incrWithTtl,  [redisKey], [rule.WindowSeconds]);

            var count = (int)res[0];
            var ttl = (int)res[1];

            var allowed = count <= rule.Limit;
            var remaining = allowed ? (rule.Limit - count) : 0;
            var retryAfter = ttl;

            return new RateLimitResult
            {
                Allowed = allowed,
                Remaining = remaining,
                RetryAfterSeconds = retryAfter,
                CurrentCount = count
            };
        }

        public async Task<RateLimitResult> PeekAsync(string key, RateLimitRule rule)
        {
            var redisKey = (RedisKey)BuildKey(key, rule);
            var count = await _redis.StringGetAsync(redisKey);
            var ttl = await _redis.KeyTimeToLiveAsync(redisKey);
            var c = count.HasValue ? (int)count : 0;
            var t = ttl.HasValue ? (int)ttl.Value.TotalSeconds : 0;

            var allowed = c < rule.Limit;
            var remaining = allowed ? (rule.Limit - c) : 0;

            return new RateLimitResult
            {
                Allowed = allowed,
                Remaining = remaining,
                RetryAfterSeconds = t,
                CurrentCount = c
            };
        }
    }
}
