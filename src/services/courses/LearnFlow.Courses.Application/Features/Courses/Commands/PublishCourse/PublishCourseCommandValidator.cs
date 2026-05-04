using FluentValidation;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.PublishCourse;

public class PublishCourseCommandValidator : AbstractValidator<PublishCourseCommand>
{
    public PublishCourseCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.InstructorId).NotEmpty();
    }
}