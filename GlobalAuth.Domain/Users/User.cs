using GlobalAuth.Domain.Enums;
using GlobalAuth.Domain.Tokens;
using GlobalAuth.Domain.Common;
using GlobalAuth.Domain.Applications;

namespace GlobalAuth.Domain.Users
{
    public class User : BaseEntity
    {
        private string _email = default!;
        private string _normalizedEmail = default!;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                _normalizedEmail = value?.Trim().ToUpperInvariant() ?? string.Empty;
            }
        }

        public string NormalizedEmail
        {
            get => _normalizedEmail;
            private set => _normalizedEmail = value;
        }

        public string PasswordHash { get; set; } = default!;
        public UserRole Role { get; set; } = UserRole.User;

        public bool IsActive { get; set; } = true;
        public bool IsEmailConfirmed { get; set; } = false;

        public DateTime? PasswordChangedAtUtc { get; set; }
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();
        public DateTime? LastLoginUtc { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntilUtc { get; set; }

        // Relacje
        public ICollection<UserApplication> UserApplications { get; set; } = new List<UserApplication>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
