using FluentValidation;

namespace LearnFlow.Enrollments.Application.Features.Enrollments.Commands.CancelEnrollment;

public class CancelEnrollmentCommandValidator : AbstractValidator<CancelEnrollmentCommand>
{
    public CancelEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}