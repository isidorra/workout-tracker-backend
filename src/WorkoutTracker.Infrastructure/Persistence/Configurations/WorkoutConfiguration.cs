using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WorkoutTracker.Application.Features.Workouts;
using WorkoutTracker.Domain.Workouts;
using WorkoutTracker.Infrastructure.Identity;

namespace WorkoutTracker.Infrastructure.Persistence.Configurations;

public sealed class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .ValueGeneratedNever();

        // Npgsql maps DateTime to timestamptz by default, which only accepts UTC values.
        builder.Property(w => w.PerformedAt)
            .HasColumnType("timestamp without time zone");

        builder.Property(w => w.Notes)
            .HasMaxLength(WorkoutPolicy.MaxNotesLength);

        builder.HasIndex(w => new { w.UserId, w.PerformedAt });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // The snake_case naming convention does not rename check constraints, so names and SQL use column names.
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_workouts_duration_minutes", "duration_minutes > 0");
            t.HasCheckConstraint("ck_workouts_calories", "calories >= 0");
            t.HasCheckConstraint("ck_workouts_difficulty", "difficulty BETWEEN 1 AND 10");
            t.HasCheckConstraint("ck_workouts_fatigue", "fatigue BETWEEN 1 AND 10");
        });
    }
}
