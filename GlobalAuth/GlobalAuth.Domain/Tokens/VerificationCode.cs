using GlobalAuth.Domain.Enums;

namespace GlobalAuth.Domain.Tokens
{
    public class VerificationCode
    {
        public string Code { get; set; } = default!;
        public Guid UserId { get; set; }
        public UserTokenPurpose Purpose { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAtUtc { get; set; }
        public bool Used { get; set; } = false;
    }
}
