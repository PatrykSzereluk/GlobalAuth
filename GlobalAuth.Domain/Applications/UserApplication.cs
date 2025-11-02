using GlobalAuth.Domain.Common;

namespace GlobalAuth.Domain.Applications
{
    public class UserApplication : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ApplicationClientId { get; set; }

        public bool IsEnabled { get; set; } = true;
        public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastAccessedAtUtc { get; set; }
    }
}
