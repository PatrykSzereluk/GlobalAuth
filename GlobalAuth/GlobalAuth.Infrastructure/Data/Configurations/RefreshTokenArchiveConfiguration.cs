using GlobalAuth.Domain.Users;
using GlobalAuth.Domain.Tokens;
using Microsoft.EntityFrameworkCore;
using GlobalAuth.Domain.Applications;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlobalAuth.Infrastructure.Data.Configurations
{
    public class RefreshTokenArchiveConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokenArchive");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(r => r.Device).HasMaxLength(100);
            builder.Property(r => r.IpAddress).HasMaxLength(45);
            builder.Property(r => r.UserAgent).HasMaxLength(255);

            builder.Property(r => r.CreatedAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder
                .HasOne<User>()
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne<ApplicationClient>()
                .WithMany()
                .HasForeignKey(r => r.ApplicationClientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
