using FluentValidation;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.AddLesson;

public class AddLessonCommandValidator : AbstractValidator<AddLessonCommand>
{
    public AddLessonCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.ModuleId).NotEmpty();
        RuleFor(x => x.InstructorId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.VideoUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.Order).GreaterThan(0);
    }
}