using GlobalAuth.Domain.Enums;
using GlobalAuth.Domain.Common;

namespace GlobalAuth.Domain.Users
{
    public class UserToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public UserTokenPurpose Purpose { get; set; }
        public string Token { get; set; } = default!;
        public DateTime ExpiresAtUtc { get; set; }
        public bool IsUsed { get; set; }
        public bool IsExpired() => DateTime.UtcNow > ExpiresAtUtc;
    }
}
