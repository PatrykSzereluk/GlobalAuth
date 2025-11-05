namespace GlobalAuth.Application.Common.DTOs
{
    public record RefreshSessionDto(
        string Token,
        string Device,
        string IpAddress,
        string UserAgent,
        DateTime ExpiresAtUtc
    );
}
