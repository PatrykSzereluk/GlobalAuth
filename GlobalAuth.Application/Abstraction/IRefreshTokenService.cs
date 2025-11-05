using GlobalAuth.Application.Common.DTOs;

namespace GlobalAuth.Application.Abstraction
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateAsync(
        Guid userId,
        Guid appClientId,
        string device,
        string ip,
        string userAgent,
        int ttlDays = 7);

        
    }
}

