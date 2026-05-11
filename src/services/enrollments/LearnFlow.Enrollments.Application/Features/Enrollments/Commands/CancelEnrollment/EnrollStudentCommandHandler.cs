using LearnFlow.Enrollments.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Enrollments.Application.Features.Enrollments.Commands.CancelEnrollment;

public class CancelEnrollmentCommandHandler : IRequestHandler<CancelEnrollmentCommand>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILogger<CancelEnrollmentCommandHandler> _logger;

    public CancelEnrollmentCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ILogger<CancelEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _logger = logger;
    }

    public async Task Handle(CancelEnrollmentCommand command, CancellationToken ct)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(command.EnrollmentId, ct)
            ?? throw new InvalidOperationException($"Enrollment {command.EnrollmentId} not found.");

        if (enrollment.StudentId != command.StudentId)
            throw new UnauthorizedAccessException("You can only cancel your own enrollments.");

        enrollment.Cancel();
        await _enrollmentRepository.UpdateAsync(enrollment, ct);

        _logger.LogInformation(
            "Enrollment {EnrollmentId} cancelled by student {StudentId}",
            command.EnrollmentId, command.StudentId);
    }
}