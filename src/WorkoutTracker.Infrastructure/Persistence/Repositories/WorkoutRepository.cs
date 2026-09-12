using Microsoft.EntityFrameworkCore;

using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Infrastructure.Persistence.Repositories;

public sealed class WorkoutRepository(AppDbContext context) : IWorkoutRepository
{
    public Task<Workout> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return context.Workouts.SingleOrDefaultAsync(w => w.Id == id && w.UserId == userId, cancellationToken);
    }

    public async Task<(IReadOnlyList<Workout> Items, int TotalCount)> GetByUserAsync(
        Guid userId,
        WorkoutType? type,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var filtered = context.Workouts
            .AsNoTracking()
            .Where(w => w.UserId == userId);

        if (type.HasValue)
        {
            filtered = filtered.Where(w => w.Type == type.Value);
        }

        var totalCount = await filtered.CountAsync(cancellationToken);
        var items = await filtered
            .OrderByDescending(w => w.PerformedAt)
            .ThenByDescending(w => w.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Workout> GetLastByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return context.Workouts
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.PerformedAt)
            .ThenByDescending(w => w.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Workout>> GetByUserInRangeAsync(
        Guid userId,
        DateTime fromInclusive,
        DateTime toExclusive,
        CancellationToken cancellationToken = default)
    {
        return await context.Workouts
            .AsNoTracking()
            .Where(w =>
                w.UserId == userId &&
                w.PerformedAt >= fromInclusive &&
                w.PerformedAt < toExclusive)
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
