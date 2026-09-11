using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Features.Workouts;

public sealed record GetWorkoutsQuery();

public static class GetWorkoutsHandler
{
    public static async Task<IReadOnlyList<WorkoutResponse>> Handle(
        GetWorkoutsQuery query,
        ICurrentUser currentUser,
        IWorkoutRepository workoutRepository,
        CancellationToken cancellationToken)
    {
        var workouts = await workoutRepository.GetByUserAsync(currentUser.UserId, cancellationToken);

        return workouts.Select(workout => workout.ToResponse()).ToList();
    }
}
