using NSubstitute;

using WorkoutTracker.Application.Common.Exceptions;
using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Application.Features.Workouts;
using WorkoutTracker.Domain.Common;
using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Tests.Workouts;

public sealed class WorkoutHandlerTests
{
    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid WorkoutId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task Get_passes_current_user_id_and_throws_when_repository_misses()
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(UserId);

        var repository = Substitute.For<IWorkoutRepository>();
        repository.GetByIdAsync(WorkoutId, UserId, Arg.Any<CancellationToken>())
            .Returns((Workout)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            GetWorkoutHandler.Handle(new GetWorkoutQuery(WorkoutId), currentUser, repository, CancellationToken.None));

        await repository.Received(1).GetByIdAsync(WorkoutId, UserId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_passes_current_user_id_and_throws_when_repository_misses()
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(UserId);

        var repository = Substitute.For<IWorkoutRepository>();
        repository.GetByIdAsync(WorkoutId, UserId, Arg.Any<CancellationToken>())
            .Returns((Workout)null);

        var command = new UpdateWorkoutCommand(
            WorkoutId,
            WorkoutType.Cardio,
            new DateTime(2026, 3, 2, 10, 0, 0, DateTimeKind.Unspecified),
            30,
            200,
            5,
            5,
            "notes");

        await Assert.ThrowsAsync<NotFoundException>(() =>
            UpdateWorkoutHandler.Handle(
                command,
                currentUser,
                repository,
                Substitute.For<IUnitOfWork>(),
                TimeProvider.System,
                CancellationToken.None));

        await repository.Received(1).GetByIdAsync(WorkoutId, UserId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_passes_current_user_id_and_throws_when_repository_misses()
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(UserId);

        var repository = Substitute.For<IWorkoutRepository>();
        repository.GetByIdAsync(WorkoutId, UserId, Arg.Any<CancellationToken>())
            .Returns((Workout)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            DeleteWorkoutHandler.Handle(
                new DeleteWorkoutCommand(WorkoutId),
                currentUser,
                repository,
                Substitute.For<IUnitOfWork>(),
                CancellationToken.None));

        await repository.Received(1).GetByIdAsync(WorkoutId, UserId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_maps_current_user_normalizes_blank_notes_and_uses_time_provider()
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(UserId);

        Workout added = null;
        var repository = Substitute.For<IWorkoutRepository>();
        repository.When(repo => repo.Add(Arg.Any<Workout>()))
            .Do(call => added = call.Arg<Workout>());

        var unitOfWork = Substitute.For<IUnitOfWork>();
        var now = new DateTimeOffset(2026, 3, 12, 8, 0, 0, TimeSpan.Zero);
        var timeProvider = new FixedTimeProvider(now);

        var command = new CreateWorkoutCommand(
            WorkoutType.Flexibility,
            new DateTime(2026, 3, 2, 10, 0, 0, DateTimeKind.Unspecified),
            40,
            180,
            4,
            3,
            "   ");

        var response = await CreateWorkoutHandler.Handle(
            command,
            currentUser,
            repository,
            unitOfWork,
            timeProvider,
            CancellationToken.None);

        Assert.NotNull(added);
        Assert.Equal(UserId, added.UserId);
        Assert.Null(added.Notes);
        Assert.Equal(now, added.CreatedAt);
        Assert.Equal(now, added.UpdatedAt);
        Assert.Equal(added.Id, response.Id);
        Assert.Null(response.Notes);

        repository.Received(1).Add(added);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return utcNow;
        }
    }
}
