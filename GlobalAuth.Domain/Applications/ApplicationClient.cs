using GlobalAuth.Domain.Enums;
using GlobalAuth.Domain.Common;

namespace GlobalAuth.Domain.Applications
{
    public class ApplicationClient : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string ClientId { get; set; } = Guid.NewGuid().ToString("N");
        public string ClientSecretHash { get; set; } = default!;
        public ApplicationClientType ClientType { get; set; }
        public ApplicationClientStatus Status { get; set; } = ApplicationClientStatus.Active;

        // Relacje
        public ICollection<UserApplication> UserApplications { get; set; } = new List<UserApplication>();
    }
}
