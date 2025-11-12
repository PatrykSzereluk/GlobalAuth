using System.Net;
using Microsoft.Extensions.Options;
using GlobalAuth.Application.Common;
using GlobalAuth.Application.Abstraction;
using GlobalAuth.Application.Common.RateLimiter;
using GlobalAuth.Application.Common.VerificationOptions;

namespace GlobalAuth.Api.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly string _redisPolicyPrefix = "login:ip";
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RequestDelegate _next;
        private readonly RateLimitRule LoginRule;

        public RateLimitingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory, IOptions<RateLimitOptions> options, IOptions<VerificationCodeOption> codes)
        {
            _next = next;
            _scopeFactory = scopeFactory;
            LoginRule = options.Value.Rules.Where(t => t.Policy == _redisPolicyPrefix).FirstOrDefault()!;
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

                context.Response.Headers[RequestHeaders.XRateLimitLimit] = LoginRule.Limit.ToString();
                context.Response.Headers[RequestHeaders.XRateLimitRemaining] = result.Remaining.ToString();
                context.Response.Headers[RequestHeaders.XRateLimitReset] = result.RetryAfterSeconds.ToString();

                if (!result.Allowed)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.Headers[RequestHeaders.RetryAfter] = result.RetryAfterSeconds.ToString();
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
