using MediatR;

namespace LearnFlow.Progress.Application.Features.Progress.Commands.MarkLessonComplete;

public record MarkLessonCompleteCommand(
    string StudentId,
    string CourseId,
    string ModuleId,
    string LessonId
) : IRequest;