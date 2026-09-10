using WorkoutTracker.Domain.Common;

namespace WorkoutTracker.Infrastructure.Persistence;

public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
