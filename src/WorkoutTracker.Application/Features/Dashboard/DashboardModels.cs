using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Features.Dashboard;

public static class DashboardPolicy
{
    public const int MinYear = 2000;
    public const int MaxYear = 2100;
}

public sealed record LastWorkoutSummary(
    Guid Id,
    WorkoutType Type,
    DateTime PerformedAt,
    int DurationMinutes);

public sealed record WeekStats(
    int WorkoutCount,
    int TotalDurationMinutes,
    double? AverageDifficulty,
    double? AverageFatigue);

internal static class DashboardCalendar
{
    public static DateOnly StartOfWeek(DateOnly day)
    {
        var daysFromMonday = ((int)day.DayOfWeek + 6) % 7;

        return day.AddDays(-daysFromMonday);
    }

    public static WeekStats ToWeekStats(IReadOnlyCollection<Workout> workouts)
    {
        var workoutCount = workouts.Count;

        if (workoutCount == 0)
        {
            return new WeekStats(0, 0, null, null);
        }

        return new WeekStats(
            workoutCount,
            workouts.Sum(workout => workout.DurationMinutes),
            workouts.Average(workout => (double)workout.Difficulty),
            workouts.Average(workout => (double)workout.Fatigue));
    }
}
