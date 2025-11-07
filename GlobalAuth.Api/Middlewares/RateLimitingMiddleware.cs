using System.Net;
using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Common.RateLimiter;

namespace GlobalAuth.Api.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly RateLimitRule LoginRule = new()
        {
            Limit = 5,
            WindowSeconds = 60,
            Policy = "login:ip"
        };

        public RateLimitingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

            if (path.Contains("/api/auth/login") ||
                path.Contains("/api/auth/register"))
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                using var scope = _scopeFactory.CreateScope();
                var rateLimiter = scope.ServiceProvider.GetRequiredService<ICustomRateLimiter>();

                var result = await rateLimiter.IncrementAndCheckAsync(ip, LoginRule);

                context.Response.Headers["X-RateLimit-Limit"] = LoginRule.Limit.ToString();
                context.Response.Headers["X-RateLimit-Remaining"] = result.Remaining.ToString();
                context.Response.Headers["X-RateLimit-Reset"] = result.RetryAfterSeconds.ToString();

                if (!result.Allowed)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.Headers["Retry-After"] = result.RetryAfterSeconds.ToString();
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Too many requests. Please try again later.",
                        retryAfterSeconds = result.RetryAfterSeconds
                    });
                    return;
                }
            }

            await _next(context);
        }
    }
}
