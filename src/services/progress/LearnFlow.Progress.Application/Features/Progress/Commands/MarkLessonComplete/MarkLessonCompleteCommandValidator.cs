using FluentValidation;

namespace LearnFlow.Progress.Application.Features.Progress.Commands.MarkLessonComplete;

public class MarkLessonCompleteCommandValidator : AbstractValidator<MarkLessonCompleteCommand>
{
    public MarkLessonCompleteCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.ModuleId).NotEmpty();
        RuleFor(x => x.LessonId).NotEmpty();
    }
}