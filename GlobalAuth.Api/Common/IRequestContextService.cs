namespace GlobalAuth.Api.Common
{
    public interface IRequestContextService
    {
        string GetClientIp();
        string GetUserAgent();
        string GetDeviceDescription();
    }
}
