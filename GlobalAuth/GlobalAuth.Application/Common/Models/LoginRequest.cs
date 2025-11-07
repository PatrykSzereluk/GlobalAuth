namespace GlobalAuth.Application.Common.Models
{
    public record LoginRequest(string Email, string Password, string ClientId);
}
