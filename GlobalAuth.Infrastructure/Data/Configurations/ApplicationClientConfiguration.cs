using Microsoft.EntityFrameworkCore;
using GlobalAuth.Domain.Applications;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlobalAuth.Infrastructure.Data.Configurations
{
    public class ApplicationClientConfiguration : IEntityTypeConfiguration<ApplicationClient>
    {
        public void Configure(EntityTypeBuilder<ApplicationClient> builder)
        {
            builder.ToTable("ApplicationClients");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ClientId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(c => c.ClientSecretHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.ClientType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.HasIndex(c => c.ClientId).IsUnique();
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.ClientType);

            builder.Property(c => c.CreatedAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
