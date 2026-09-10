using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WorkoutTracker.Infrastructure.Identity;

namespace WorkoutTracker.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("users");

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.NormalizedUserName)
            .HasDatabaseName("ix_users_normalized_user_name")
            .IsUnique();

        builder.HasIndex(u => u.NormalizedEmail)
            .HasDatabaseName("ix_users_normalized_email")
            .IsUnique();
    }
}
