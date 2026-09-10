using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WorkoutTracker.Domain.Auth;
using WorkoutTracker.Infrastructure.Identity;

namespace WorkoutTracker.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(t => t.TokenHash)
            .IsUnique();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
