using FluentValidation;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.ArchiveCourse;

public class ArchiveCourseCommandValidator : AbstractValidator<ArchiveCourseCommand>
{
    public ArchiveCourseCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.InstructorId).NotEmpty();
    }
}