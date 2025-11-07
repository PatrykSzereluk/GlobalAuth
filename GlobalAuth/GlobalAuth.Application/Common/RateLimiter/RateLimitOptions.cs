namespace GlobalAuth.Application.Common.RateLimiter
{
    public class RateLimitOptions
    {
        public RateLimitRule[] Rules { get; set; } = [];
    }
}
