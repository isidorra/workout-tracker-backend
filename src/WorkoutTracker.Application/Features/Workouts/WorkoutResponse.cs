using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Features.Workouts;

public sealed record WorkoutResponse(
    Guid Id,
    WorkoutType Type,
    DateTime PerformedAt,
    int DurationMinutes,
    int Calories,
    int Difficulty,
    int Fatigue,
    string Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

internal static class WorkoutMapping
{
    public static WorkoutResponse ToResponse(this Workout workout)
    {
        return new WorkoutResponse(
            workout.Id,
            workout.Type,
            workout.PerformedAt,
            workout.DurationMinutes,
            workout.Calories,
            workout.Difficulty,
            workout.Fatigue,
            workout.Notes,
            workout.CreatedAt,
            workout.UpdatedAt);
    }
}
