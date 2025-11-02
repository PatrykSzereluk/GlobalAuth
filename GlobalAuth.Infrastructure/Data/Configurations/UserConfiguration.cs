using GlobalAuth.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlobalAuth.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.NormalizedEmail)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Role)
                .HasConversion<int>()
                .IsRequired();

            builder.HasIndex(u => u.NormalizedEmail)
                .IsUnique();

            builder.Property(u => u.SecurityStamp)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.IsEmailConfirmed)
                .HasDefaultValue(false);

            builder.Property(u => u.CreatedAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
