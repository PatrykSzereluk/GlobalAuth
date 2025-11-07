namespace GlobalAuth.Application.Abstraction
{
    public interface IVerificationCodeService
    {
        Task<string> GenerateAsync(Guid userId, string purpose);
        Task<bool> ValidateAsync(Guid userId, string purpose, string code);
        Task InvalidateAsync(Guid userId, string purpose, string code);
    }
}
