namespace GlobalAuth.Application.Common.Models
{
    public record LogoutAllFromAppRequest(Guid UserId, Guid AppClientId);
}
