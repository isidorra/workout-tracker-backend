using FluentValidation;

using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Features.Dashboard;

public sealed record GetDashboardProgressQuery
{
    public int Year { get; init; }
    public int Month { get; init; }
}

public sealed class GetDashboardProgressQueryValidator : AbstractValidator<GetDashboardProgressQuery>
{
    public GetDashboardProgressQueryValidator()
    {
        // Year and month bind to 0 when the query string is missing; that is not a real month.
        RuleFor(x => x.Year)
            .InclusiveBetween(DashboardPolicy.MinYear, DashboardPolicy.MaxYear);

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12);
    }
}

public sealed record ProgressWeek(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    int WorkoutCount,
    int TotalDurationMinutes,
    double? AverageDifficulty,
    double? AverageFatigue);

public sealed record DashboardProgressResponse(
    int Year,
    int Month,
    IReadOnlyList<ProgressWeek> Weeks);

public static class GetDashboardProgressHandler
{
    public static async Task<DashboardProgressResponse> Handle(
        GetDashboardProgressQuery query,
        ICurrentUser currentUser,
        IWorkoutRepository workoutRepository,
        CancellationToken cancellationToken)
    {
        var monthStart = new DateOnly(query.Year, query.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var firstMonday = DashboardCalendar.StartOfWeek(monthStart);
        var lastMonday = DashboardCalendar.StartOfWeek(monthEnd);
        var lastSunday = lastMonday.AddDays(6);

        var workouts = await workoutRepository.GetByUserInRangeAsync(
            currentUser.UserId,
            firstMonday.ToDateTime(TimeOnly.MinValue),
            lastSunday.AddDays(1).ToDateTime(TimeOnly.MinValue),
            cancellationToken);

        var workoutsByWeek = workouts
            .GroupBy(workout => DashboardCalendar.StartOfWeek(DateOnly.FromDateTime(workout.PerformedAt)))
            .ToDictionary(group => group.Key, group => (IReadOnlyCollection<Workout>)group.ToList());

        var weeks = new List<ProgressWeek>();

        for (var weekStart = firstMonday; weekStart <= lastMonday; weekStart = weekStart.AddDays(7))
        {
            var weekEnd = weekStart.AddDays(6);
            var weekWorkouts = workoutsByWeek.GetValueOrDefault(weekStart, Array.Empty<Workout>());
            var stats = DashboardCalendar.ToWeekStats(weekWorkouts);

            weeks.Add(new ProgressWeek(
                weekStart,
                weekEnd,
                stats.WorkoutCount,
                stats.TotalDurationMinutes,
                stats.AverageDifficulty,
                stats.AverageFatigue));
        }

        return new DashboardProgressResponse(query.Year, query.Month, weeks);
    }
}
