using GlobalAuth.Domain.Common;

namespace GlobalAuth.Domain.Tokens
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ApplicationClientId { get; set; }

        public string Token { get; set; } = default!;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? RevokedReason { get; set; }

        public string Device { get; set; } = "unknown";
        public string IpAddress { get; set; } = "N/A";
        public string UserAgent { get; set; } = "N/A";

        public bool IsExpired() => DateTime.UtcNow >= ExpiresAtUtc;
        public bool IsActive() => RevokedAtUtc is null && !IsExpired();
    }
}
