using FluentValidation;

using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Application.Common.Models;
using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Features.Workouts;

public sealed record GetWorkoutsQuery
{
    public WorkoutType? Type { get; init; }
    public int Page { get; init; } = WorkoutPolicy.DefaultPage;
    public int PageSize { get; init; } = WorkoutPolicy.DefaultPageSize;
}

public sealed class GetWorkoutsQueryValidator : AbstractValidator<GetWorkoutsQuery>
{
    public GetWorkoutsQueryValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(WorkoutPolicy.DefaultPage);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, WorkoutPolicy.MaxPageSize);
    }
}

public static class GetWorkoutsHandler
{
    public static async Task<PagedResponse<WorkoutResponse>> Handle(
        GetWorkoutsQuery query,
        ICurrentUser currentUser,
        IWorkoutRepository workoutRepository,
        CancellationToken cancellationToken)
    {
        var (workouts, totalCount) = await workoutRepository.GetByUserAsync(
            currentUser.UserId,
            query.Type,
            query.Page,
            query.PageSize,
            cancellationToken);

        return new PagedResponse<WorkoutResponse>(
            workouts.Select(workout => workout.ToResponse()).ToList(),
            query.Page,
            query.PageSize,
            totalCount);
    }
}
