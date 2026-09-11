using WorkoutTracker.Domain.Common;

namespace WorkoutTracker.Domain.Workouts;

public sealed class Workout : Entity
{
    private Workout()
    {
    }

    public Guid UserId { get; private set; }
    public WorkoutType Type { get; private set; }

    // Wall-clock time as the user entered it, with no time zone attached.
    public DateTime PerformedAt { get; private set; }

    public int DurationMinutes { get; private set; }
    public int Calories { get; private set; }
    public int Difficulty { get; private set; }
    public int Fatigue { get; private set; }
    public string Notes { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Workout Create(
        Guid userId,
        WorkoutType type,
        DateTime performedAt,
        int durationMinutes,
        int calories,
        int difficulty,
        int fatigue,
        string notes,
        DateTimeOffset now)
    {
        return new Workout
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Type = type,
            PerformedAt = performedAt,
            DurationMinutes = durationMinutes,
            Calories = calories,
            Difficulty = difficulty,
            Fatigue = fatigue,
            Notes = NormalizeNotes(notes),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Update(
        WorkoutType type,
        DateTime performedAt,
        int durationMinutes,
        int calories,
        int difficulty,
        int fatigue,
        string notes,
        DateTimeOffset now)
    {
        Type = type;
        PerformedAt = performedAt;
        DurationMinutes = durationMinutes;
        Calories = calories;
        Difficulty = difficulty;
        Fatigue = fatigue;
        Notes = NormalizeNotes(notes);
        UpdatedAt = now;
    }

    private static string NormalizeNotes(string notes)
    {
        return string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}
