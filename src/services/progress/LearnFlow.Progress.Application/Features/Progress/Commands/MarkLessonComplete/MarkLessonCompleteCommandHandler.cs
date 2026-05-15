using LearnFlow.Progress.Application.Common.Interfaces;
using LearnFlow.Shared.Contracts.Events.Progress;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Progress.Application.Features.Progress.Commands.MarkLessonComplete;

public class MarkLessonCompleteCommandHandler : IRequestHandler<MarkLessonCompleteCommand>
{
    private readonly IProgressRepository _progressRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MarkLessonCompleteCommandHandler> _logger;

    public MarkLessonCompleteCommandHandler(
        IProgressRepository progressRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<MarkLessonCompleteCommandHandler> logger)
    {
        _progressRepository = progressRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(MarkLessonCompleteCommand command, CancellationToken ct)
    {
        var progress = await _progressRepository
            .GetByStudentAndCourseAsync(command.StudentId, command.CourseId, ct)
            ?? throw new InvalidOperationException(
                $"Progress not found for student {command.StudentId} course {command.CourseId}.");

        var wasUpdated = progress.CompleteLesson(command.ModuleId, command.LessonId);

        if (!wasUpdated)
        {
            _logger.LogInformation(
                "Lesson {LessonId} already completed by student {StudentId} — skipping",
                command.LessonId, command.StudentId);
            return; // idempotent
        }

        await _progressRepository.UpdateAsync(progress, ct);

        _logger.LogInformation(
            "Lesson {LessonId} completed by student {StudentId}. Progress: {Percentage}%",
            command.LessonId, command.StudentId, progress.CompletionPercentage);

        if (progress.IsFullyCompleted)
        {
            await _publishEndpoint.Publish(new CourseCompletedEvent
            {
                ProgressId = progress.Id,
                StudentId = progress.StudentId,
                CourseId = progress.CourseId,
                CourseTitleSnapshot = string.Empty,
                CompletedAt = progress.CompletedAt!.Value
            }, ct);

            _logger.LogInformation(
                "Course {CourseId} completed by student {StudentId}",
                command.CourseId, command.StudentId);
        }
    }
}