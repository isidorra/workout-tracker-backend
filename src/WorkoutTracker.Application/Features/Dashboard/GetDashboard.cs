using FluentValidation;

using WorkoutTracker.Application.Common.Interfaces;

namespace WorkoutTracker.Application.Features.Dashboard;

public sealed record GetDashboardQuery
{
    public DateOnly Today { get; init; }
}

public sealed class GetDashboardQueryValidator : AbstractValidator<GetDashboardQuery>
{
    public GetDashboardQueryValidator()
    {
        // DateOnly binds to 0001-01-01 when the query string is missing; that is not a real "today".
        RuleFor(x => x.Today)
            .Must(today => today != default)
            .WithMessage("'{PropertyName}' must be a calendar date.");
    }
}

public sealed record DashboardResponse(
    DateOnly Today,
    DateOnly WeekStart,
    DateOnly WeekEnd,
    bool HasWorkoutToday,
    LastWorkoutSummary LastWorkout,
    WeekStats Week);

public static class GetDashboardHandler
{
    public static async Task<DashboardResponse> Handle(
        GetDashboardQuery query,
        ICurrentUser currentUser,
        IWorkoutReadQueries workoutReadQueries,
        CancellationToken cancellationToken)
    {
        var weekStart = DashboardCalendar.StartOfWeek(query.Today);
        var weekEnd = weekStart.AddDays(6);

        var weekStartAt = weekStart.ToDateTime(TimeOnly.MinValue);
        var weekEndExclusive = weekEnd.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var todayStart = query.Today.ToDateTime(TimeOnly.MinValue);
        var todayEndExclusive = query.Today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var lastWorkout = await workoutReadQueries.GetLastByUserAsync(
            currentUser.UserId,
            cancellationToken);
        var weekWorkouts = await workoutReadQueries.GetStatsByUserInRangeAsync(
            currentUser.UserId,
            weekStartAt,
            weekEndExclusive,
            cancellationToken);

        var hasWorkoutToday = weekWorkouts.Any(workout =>
            workout.PerformedAt >= todayStart && workout.PerformedAt < todayEndExclusive);

        return new DashboardResponse(
            query.Today,
            weekStart,
            weekEnd,
            hasWorkoutToday,
            lastWorkout,
            DashboardCalendar.ToWeekStats(weekWorkouts));
    }
}
