using GlobalAuth.Domain.Enums;
using GlobalAuth.Domain.Users;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using GlobalAuth.Domain.Applications;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalAuth.Infrastructure.Data
{
    public static class AuthDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AuthDbSeeder");

            await context.Database.MigrateAsync();

            if (!await context.ApplicationClients.AnyAsync())
            {
                logger.LogInformation("Adding test aplications...");

                var apps = new List<ApplicationClient>
                {
                    new()
                    {
                        Name = "WebApp",
                        ClientId = "web-app",
                        ClientSecretHash = BCrypt.Net.BCrypt.HashPassword("webapp-secret"),
                        ClientType = ApplicationClientType.UserFacing,
                        Status = ApplicationClientStatus.Active
                    },
                    new()
                    {
                        Name = "MobileApp",
                        ClientId = "mobile-app",
                        ClientSecretHash = BCrypt.Net.BCrypt.HashPassword("mobile-secret"),
                        ClientType = ApplicationClientType.UserFacing,
                        Status = ApplicationClientStatus.Active
                    },
                    new()
                    {
                        Name = "InternalService",
                        ClientId = "internal-service",
                        ClientSecretHash = BCrypt.Net.BCrypt.HashPassword("internal-secret"),
                        ClientType = ApplicationClientType.Service,
                        Status = ApplicationClientStatus.Active
                    }
                };

                await context.ApplicationClients.AddRangeAsync(apps);
                await context.SaveChangesAsync();

                logger.LogInformation("Added test aplications...");
            }

            if (!await context.Users.AnyAsync(u => u.Email == "admin@globalauth.com"))
            {
                var admin = new User
                {
                    Email = "admin@globalauth.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = UserRole.SuperAdmin,
                    IsActive = true,
                    IsEmailConfirmed = true
                };

                await context.Users.AddAsync(admin);
                await context.SaveChangesAsync();

                var webApp = await context.ApplicationClients.FirstAsync(c => c.ClientId == "web-app");

                var userApp = new UserApplication
                {
                    UserId = admin.Id,
                    ApplicationClientId = webApp.Id,
                    IsEnabled = true
                };

                await context.UserApplications.AddAsync(userApp);
                await context.SaveChangesAsync();
            }

        }
    }
}
