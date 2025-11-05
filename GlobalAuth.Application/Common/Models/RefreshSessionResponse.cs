namespace GlobalAuth.Application.Common.DTOs
{
    public record RefreshSessionResponse(
        string Token,
        string Device,
        string IpAddress,
        string UserAgent,
        DateTime ExpiresAtUtc
    );
}
