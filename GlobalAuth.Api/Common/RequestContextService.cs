namespace GlobalAuth.Api.Common
{
    public class RequestContextService : IRequestContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClientIp()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                return "unknown";

            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
                return forwarded.Split(',').First().Trim();

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        public string GetUserAgent()
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Request.Headers["User-Agent"].FirstOrDefault() ?? "unknown";
        }

        public string GetDeviceDescription()
        {
            var ua = GetUserAgent();
            if (ua == "unknown")
                return "Unknown device";

            if (ua.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                return "Windows PC";
            if (ua.Contains("Macintosh", StringComparison.OrdinalIgnoreCase))
                return "Mac";
            if (ua.Contains("Android", StringComparison.OrdinalIgnoreCase))
                return "Android Device";
            if (ua.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
                return "iPhone";
            if (ua.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                return "iPad";

            return "Other Device";
        }
    }
}
