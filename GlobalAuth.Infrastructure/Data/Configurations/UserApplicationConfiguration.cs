using GlobalAuth.Domain.Users;
using Microsoft.EntityFrameworkCore;
using GlobalAuth.Domain.Applications;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlobalAuth.Infrastructure.Data.Configurations
{
    public class UserApplicationConfiguration : IEntityTypeConfiguration<UserApplication>
    {
        public void Configure(EntityTypeBuilder<UserApplication> builder)
        {
            builder.ToTable("UserApplications");

            builder.HasKey(ua => ua.Id);

            builder.HasIndex(ua => new { ua.UserId, ua.ApplicationClientId }).IsUnique();
            builder.HasIndex(ua => ua.IsEnabled);

            builder.Property(ua => ua.IsEnabled)
                .HasDefaultValue(true);

            builder.Property(ua => ua.RegisteredAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder
                .HasOne<ApplicationClient>()
                .WithMany(c => c.UserApplications)
                .HasForeignKey(ua => ua.ApplicationClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne<User>()
                .WithMany(u => u.UserApplications)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
