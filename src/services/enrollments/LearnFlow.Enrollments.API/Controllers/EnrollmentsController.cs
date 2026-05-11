using LearnFlow.Enrollments.Application.Features.Enrollments.Commands.CancelEnrollment;
using LearnFlow.Enrollments.Application.Features.Enrollments.Commands.EnrollStudent;
using LearnFlow.Enrollments.Application.Features.Enrollments.Queries.GetCourseEnrollments;
using LearnFlow.Enrollments.Application.Features.Enrollments.Queries.GetMyEnrollments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearnFlow.Enrollments.API.Controllers;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Enroll(
        [FromBody] EnrollStudentRequest request,
        CancellationToken ct = default)
    {
        var enrollmentId = await _mediator.Send(
            new EnrollStudentCommand(request.StudentId, request.CourseId), ct);
        return StatusCode(201, new { enrollmentId });
    }

    [HttpDelete("{enrollmentId}")]
    public async Task<IActionResult> CancelEnrollment(
        string enrollmentId,
        [FromBody] CancelEnrollmentRequest request,
        CancellationToken ct = default)
    {
        await _mediator.Send(
            new CancelEnrollmentCommand(enrollmentId, request.StudentId), ct);
        return NoContent();
    }

    [HttpGet("my/{studentId}")]
    public async Task<IActionResult> GetMyEnrollments(
        string studentId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetMyEnrollmentsQuery(studentId), ct);
        return Ok(result);
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetCourseEnrollments(
        string courseId,
        [FromQuery] string instructorId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetCourseEnrollmentsQuery(courseId, instructorId), ct);
        return Ok(result);
    }
}