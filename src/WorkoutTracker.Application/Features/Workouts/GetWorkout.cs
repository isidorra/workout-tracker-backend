using WorkoutTracker.Application.Common.Exceptions;
using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Features.Workouts;

public sealed record GetWorkoutQuery(Guid Id);

public static class GetWorkoutHandler
{
    public static async Task<WorkoutResponse> Handle(
        GetWorkoutQuery query,
        ICurrentUser currentUser,
        IWorkoutRepository workoutRepository,
        CancellationToken cancellationToken)
    {
        var workout = await workoutRepository.GetByIdAsync(query.Id, currentUser.UserId, cancellationToken);

        if (workout is null)
        {
            throw new NotFoundException("Workout not found");
        }

        return workout.ToResponse();
    }
}
