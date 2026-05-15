using LearnFlow.Progress.Application.Features.Progress.Commands.MarkLessonComplete;
using LearnFlow.Progress.Application.Features.Progress.Queries.GetProgress;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearnFlow.Progress.API.Controllers;

[ApiController]
[Route("api/progress")]
public class ProgressController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProgressController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("lessons/complete")]
    public async Task<IActionResult> MarkLessonComplete(
        [FromBody] MarkLessonCompleteCommand command,
        CancellationToken ct = default)
    {
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpGet("{studentId}/{courseId}")]
    public async Task<IActionResult> GetProgress(
        string studentId,
        string courseId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetProgressQuery(studentId, courseId), ct);
        return Ok(result);
    }
}