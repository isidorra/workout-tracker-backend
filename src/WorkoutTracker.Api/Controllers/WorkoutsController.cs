using Microsoft.AspNetCore.Mvc;

using Wolverine;

using WorkoutTracker.Application.Common.Models;
using WorkoutTracker.Application.Features.Workouts;

namespace WorkoutTracker.Api.Controllers;

[ApiController]
[Route("api/workouts")]
public sealed class WorkoutsController(IMessageBus bus) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<WorkoutResponse>>> GetAll(
        [FromQuery] GetWorkoutsQuery query,
        CancellationToken cancellationToken)
    {
        var workouts = await bus.InvokeAsync<PagedResponse<WorkoutResponse>>(query, cancellationToken);

        return Ok(workouts);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkoutResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var workout = await bus.InvokeAsync<WorkoutResponse>(new GetWorkoutQuery(id), cancellationToken);

        return Ok(workout);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutResponse>> Create(CreateWorkoutCommand command, CancellationToken cancellationToken)
    {
        var workout = await bus.InvokeAsync<WorkoutResponse>(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = workout.Id }, workout);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<WorkoutResponse>> Update(
        Guid id,
        UpdateWorkoutCommand command,
        CancellationToken cancellationToken)
    {
        // The route is the source of truth for which workout is updated; any id in the body is ignored.
        var workout = await bus.InvokeAsync<WorkoutResponse>(command with { Id = id }, cancellationToken);

        return Ok(workout);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await bus.InvokeAsync(new DeleteWorkoutCommand(id), cancellationToken);

        return NoContent();
    }
}
