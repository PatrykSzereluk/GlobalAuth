using GlobalAuth.Domain.Applications;
using GlobalAuth.Domain.Tokens;
using GlobalAuth.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace GlobalAuth.Infrastructure.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<ApplicationClient> ApplicationClients => Set<ApplicationClient>();
        public DbSet<UserApplication> UserApplications => Set<UserApplication>();
        public DbSet<RefreshToken> RefreshTokenArchive => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
