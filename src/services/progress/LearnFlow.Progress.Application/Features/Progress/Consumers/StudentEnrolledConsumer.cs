using LearnFlow.Progress.Application.Common.Interfaces;
using LearnFlow.Progress.Domain.Entities;
using LearnFlow.Shared.Contracts.Events.Enrollments;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Progress.Application.Features.Progress.Consumers;

public class StudentEnrolledConsumer : IConsumer<StudentEnrolledEvent>
{
    private readonly IProgressRepository _progressRepository;
    private readonly ICourseStructureRepository _courseStructureRepository;
    private readonly ILogger<StudentEnrolledConsumer> _logger;

    public StudentEnrolledConsumer(
        IProgressRepository progressRepository,
        ICourseStructureRepository courseStructureRepository,
        ILogger<StudentEnrolledConsumer> logger)
    {
        _progressRepository = progressRepository;
        _courseStructureRepository = courseStructureRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StudentEnrolledEvent> context)
    {
        var message = context.Message;

        var existing = await _progressRepository
            .GetByStudentAndCourseAsync(message.StudentId, message.CourseId,
                context.CancellationToken);

        if (existing is not null)
        {
            _logger.LogWarning(
                "Progress already exists for student {StudentId} course {CourseId} — skipping",
                message.StudentId, message.CourseId);
            return;
        }

        var structure = await _courseStructureRepository
            .GetByCourseIdAsync(message.CourseId, context.CancellationToken);

        if (structure is null)
        {
            _logger.LogError(
                "Course structure not found for course {CourseId}. Cannot create progress.",
                message.CourseId);
            throw new InvalidOperationException(
                $"Course structure not found for course {message.CourseId}");
        }

        var modules = structure.Modules
            .Select(m => (m.ModuleId, m.LessonIds))
            .ToList();

        var progress = Domain.Entities.Progress.Create(
            message.StudentId,
            message.CourseId,
            modules);

        await _progressRepository.InsertAsync(progress, context.CancellationToken);

        _logger.LogInformation(
            "Progress created for student {StudentId} course {CourseId} with {TotalLessons} lessons",
            message.StudentId, message.CourseId, progress.TotalLessons);
    }
}