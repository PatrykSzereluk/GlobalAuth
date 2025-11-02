using GlobalAuth.Domain.Users;
using GlobalAuth.Domain.Tokens;
using Microsoft.EntityFrameworkCore;
using GlobalAuth.Domain.Applications;
using GlobalAuth.Application.Abstraction.Repositories;

namespace GlobalAuth.Infrastructure.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public IRepository<User> Users { get; }
        public IRepository<ApplicationClient> ApplicationClients { get; }
        public IRepository<UserApplication> UserApplications { get; }
        public IRepository<RefreshToken> RefreshTokenArchive { get; }

        public UnitOfWork(DbContext context)
        {
            _context = context;
            Users = new Repository<User>(_context);
            ApplicationClients = new Repository<ApplicationClient>(_context);
            UserApplications = new Repository<UserApplication>(_context);
            RefreshTokenArchive = new Repository<RefreshToken>(_context);
        }
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public async ValueTask DisposeAsync() => await _context.DisposeAsync();
    }
}
