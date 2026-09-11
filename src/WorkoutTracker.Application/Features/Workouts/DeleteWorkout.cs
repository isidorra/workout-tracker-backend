using WorkoutTracker.Application.Common.Exceptions;
using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Common;
using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Features.Workouts;

public sealed record DeleteWorkoutCommand(Guid Id);

public static class DeleteWorkoutHandler
{
    public static async Task Handle(
        DeleteWorkoutCommand command,
        ICurrentUser currentUser,
        IWorkoutRepository workoutRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var workout = await workoutRepository.GetByIdAsync(command.Id, currentUser.UserId, cancellationToken);

        if (workout is null)
        {
            throw new NotFoundException("Workout not found");
        }

        workoutRepository.Remove(workout);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
