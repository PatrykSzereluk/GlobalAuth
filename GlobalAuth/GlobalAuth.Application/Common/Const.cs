namespace GlobalAuth.Application.Common
{
    public static class Const
    {
        public static readonly string Unknown = "Unknown";
        public static readonly string UnknownDevice = "Unknown device";
        public static readonly string OtherDevice = "Other Device";
    }

    public static class RedisKeyNames
    {
        public static readonly string Refresh = "refresh"; 
        public static readonly string Sessions = "sessions"; 
        public static readonly string AllToken = "allToken"; 
    }

    public static class RefreshTokenRevokeReasons
    {
        public const string LogoutAll = "logout_all";
        public const string LogoutAllForApp = "logout_all_for_app";
        public const string Revoked = "revoked";
        public const string Logout = "logout";
        
    }

    public static class RequestHeaders
    {
        public static readonly string XForwardedFor = "X-Forwarded-For";
        public static readonly string XRealIP = "X-Real-IP";
        public static readonly string UserAgent = "User-Agent";
    }
}
