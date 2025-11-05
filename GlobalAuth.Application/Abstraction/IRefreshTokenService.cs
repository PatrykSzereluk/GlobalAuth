using GlobalAuth.Application.Common;
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

        Task<bool> ValidateAsync(Guid userId, Guid appClientId, string token);

        Task<string> RotateAsync(
            Guid userId,
            Guid appClientId,
            string oldToken,
            string device,
            string ip,
            string userAgent,
            int ttlDays = 7);

        Task RevokeAsync(Guid userId, Guid appClientId, string token, string reason = RefreshTokenRevokeReasons.Revoked);

        Task RevokeAllForAppAsync(Guid userId, Guid appClientId, string reason = RefreshTokenRevokeReasons.LogoutAllForApp);

        Task RevokeAllAsync(Guid userId, string reason = RefreshTokenRevokeReasons.LogoutAll);

        Task<IReadOnlyList<RefreshSessionResponse>> GetActiveSessionsAsync(Guid userId, Guid appClientId);
    }
}

