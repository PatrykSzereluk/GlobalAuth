using GlobalAuth.Application.Common;

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
                return Const.Unknown;

            var forwarded = context.Request.Headers[RequestHeaders.XForwardedFor].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
                return forwarded.Split(',').First().Trim();

            var realIp = context.Request.Headers[RequestHeaders.XRealIP].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
                return realIp;

            return context.Connection.RemoteIpAddress?.ToString() ?? Const.Unknown;
        }

        public string GetUserAgent()
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Request.Headers[RequestHeaders.UserAgent].FirstOrDefault() ?? Const.Unknown;
        }

        public string GetDeviceDescription()
        {
            var ua = GetUserAgent();
            if (ua == Const.Unknown)
                return Const.UnknownDevice;

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

            return Const.OtherDevice;
        }
    }
}
