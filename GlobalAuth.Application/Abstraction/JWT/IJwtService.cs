using GlobalAuth.Domain.Users;

namespace GlobalAuth.Application.Abstraction.JWT
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, string? clientId = null, string? applicationName = null);
    }
}
