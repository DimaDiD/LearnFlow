using LearnFlow.Courses.API.Controllers.Requests;
using LearnFlow.Courses.Application.Features.Courses.Commands.AddLesson;
using LearnFlow.Courses.Application.Features.Courses.Commands.AddModule;
using LearnFlow.Courses.Application.Features.Courses.Commands.ArchiveCourse;
using LearnFlow.Courses.Application.Features.Courses.Commands.CreateCourse;
using LearnFlow.Courses.Application.Features.Courses.Commands.PublishCourse;
using LearnFlow.Courses.Application.Features.Courses.Commands.UpdateCourse;
using LearnFlow.Courses.Application.Features.Courses.Queries.GetCourseById;
using LearnFlow.Courses.Application.Features.Courses.Queries.GetCourses;
using LearnFlow.Courses.Application.Features.Courses.Queries.GetInstructorCourses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearnFlow.Courses.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ── Queries ──────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCoursesQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetCourseById(
        string courseId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCourseByIdQuery(courseId), ct);
        return Ok(result);
    }

    [HttpGet("instructor/{instructorId}")]
    public async Task<IActionResult> GetInstructorCourses(
        string instructorId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetInstructorCoursesQuery(instructorId), ct);
        return Ok(result);
    }

    // ── Commands ─────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> CreateCourse(
        [FromBody] CreateCourseCommand command,
        CancellationToken ct = default)
    {
        var courseId = await _mediator.Send(command, ct);
        return StatusCode(201, new { courseId });
    }

    [HttpPut("{courseId}")]
    public async Task<IActionResult> UpdateCourse(
    string courseId,
    [FromBody] UpdateCourseRequest request,
    CancellationToken ct = default)
    {
        await _mediator.Send(new UpdateCourseCommand(
            courseId,
            request.InstructorId,
            request.Title,
            request.Description,
            request.Category,
            request.Level,
            request.Tags,
            request.Price), ct);
        return NoContent();
    }

    [HttpPost("{courseId}/modules")]
    public async Task<IActionResult> AddModule(
    string courseId,
    [FromBody] AddModuleRequest request,
    CancellationToken ct = default)
    {
        var moduleId = await _mediator.Send(new AddModuleCommand(
            courseId,
            request.InstructorId,
            request.Title,
            request.Description,
            request.Order), ct);
        return StatusCode(201, new { moduleId });
    }

    [HttpPost("{courseId}/modules/{moduleId}/lessons")]
    public async Task<IActionResult> AddLesson(
    string courseId,
    string moduleId,
    [FromBody] AddLessonRequest request,
    CancellationToken ct = default)
    {
        var lessonId = await _mediator.Send(new AddLessonCommand(
            courseId,
            moduleId,
            request.InstructorId,
            request.Title,
            request.Description,
            request.VideoUrl,
            request.DurationMinutes,
            request.Order), ct);
        return StatusCode(201, new { lessonId });
    }

    [HttpPost("{courseId}/publish")]
    public async Task<IActionResult> PublishCourse(
    string courseId,
    [FromBody] PublishCourseRequest request,
    CancellationToken ct = default)
    {
        await _mediator.Send(new PublishCourseCommand(courseId, request.InstructorId), ct);
        return NoContent();
    }

    [HttpPost("{courseId}/archive")]
    public async Task<IActionResult> ArchiveCourse(
    string courseId,
    [FromBody] ArchiveCourseRequest request,
    CancellationToken ct = default)
    {
        await _mediator.Send(new ArchiveCourseCommand(courseId, request.InstructorId), ct);
        return NoContent();
    }
}