using GlobalAuth.Domain.Enums;

namespace GlobalAuth.Application.Abstraction
{
    public interface IVerificationCodeService
    {
        Task<string> GenerateAsync(Guid userId, UserTokenPurpose purpose);
        Task<bool> ValidateAsync(Guid userId, UserTokenPurpose purpose, string code);
        Task InvalidateAsync(Guid userId, UserTokenPurpose purpose, string code);
    }
}
