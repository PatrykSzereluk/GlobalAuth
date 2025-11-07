namespace GlobalAuth.Application.Common.Models
{
    public record LogoutRequest(Guid UserId, Guid AppClientId, string Token);
}
