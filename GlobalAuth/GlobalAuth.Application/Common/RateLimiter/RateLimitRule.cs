namespace GlobalAuth.Application.Common.RateLimiter
{
    public class RateLimitRule
    {
        public int Limit { get; init; }
        public int TimeOfBlock { get; init; }
        public int WindowSeconds { get; init; }
        public required string Policy { get; init; }
    }
}
