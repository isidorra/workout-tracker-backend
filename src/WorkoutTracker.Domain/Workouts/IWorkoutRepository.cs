namespace WorkoutTracker.Domain.Workouts;

public interface IWorkoutRepository
{
    Task<Workout> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Workout>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    void Add(Workout workout);
    void Remove(Workout workout);
}
