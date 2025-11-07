using GlobalAuth.Domain.Users;
using GlobalAuth.Domain.Tokens;
using GlobalAuth.Domain.Applications;

namespace GlobalAuth.Application.Abstraction.Repositories
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<User> Users { get; }
        IRepository<ApplicationClient> ApplicationClients { get; }
        IRepository<UserApplication> UserApplications { get; }
        IRepository<RefreshToken> RefreshTokenArchive { get; }

        Task<int> SaveChangesAsync();
    }
}
