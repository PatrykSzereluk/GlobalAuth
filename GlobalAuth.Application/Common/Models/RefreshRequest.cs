namespace GlobalAuth.Application.Common.Models
{
    public record RefreshRequest(Guid UserId, Guid AppClientId, string RefreshToken);
}
