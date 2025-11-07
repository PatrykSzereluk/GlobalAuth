using GlobalAuth.Application.Common.RateLimiter;

namespace GlobalAuth.Application.Abstraction
{
    public interface ICustomRateLimiter
    {
        Task<RateLimitResult> IncrementAndCheckAsync(string key, RateLimitRule rule);
        Task<RateLimitResult> PeekAsync(string key, RateLimitRule rule);
    }
}
