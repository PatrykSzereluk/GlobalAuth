namespace GlobalAuth.Application.Common.RateLimiter
{
    public sealed class RateLimitResult
    {
        public bool Allowed { get; init; }
        public int Remaining { get; init; }
        public int RetryAfterSeconds { get; init; }
        public long CurrentCount { get; init; }
    }
}
