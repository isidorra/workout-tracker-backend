using Microsoft.EntityFrameworkCore;

using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Infrastructure.Persistence.Repositories;

public sealed class WorkoutRepository(AppDbContext context) : IWorkoutRepository
{
    public Task<Workout> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return context.Workouts.SingleOrDefaultAsync(w => w.Id == id && w.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Workout>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Workouts
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.PerformedAt)
            .ThenByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public void Add(Workout workout)
    {
        context.Workouts.Add(workout);
    }

    public void Remove(Workout workout)
    {
        context.Workouts.Remove(workout);
    }
}
