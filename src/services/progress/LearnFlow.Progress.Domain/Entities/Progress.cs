using LearnFlow.Progress.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Progress.Domain.Entities;

public class Progress
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }

    [BsonElement("studentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string StudentId { get; private set; }

    [BsonElement("courseId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CourseId { get; private set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public ProgressStatus Status { get; private set; }

    [BsonElement("completionPercentage")]
    public double CompletionPercentage { get; private set; }

    [BsonElement("totalLessons")]
    public int TotalLessons { get; private set; }

    [BsonElement("completedLessonsCount")]
    public int CompletedLessonsCount { get; private set; }

    [BsonElement("moduleProgresses")]
    public List<ModuleProgress> ModuleProgresses { get; private set; }

    [BsonElement("startedAt")]
    public DateTime StartedAt { get; private set; }

    [BsonElement("completedAt")]
    public DateTime? CompletedAt { get; private set; }

    [BsonElement("lastUpdatedAt")]
    public DateTime LastUpdatedAt { get; private set; }

    private Progress() { }

    public static Progress Create(
        string studentId,
        string courseId,
        List<(string ModuleId, List<string> LessonIds)> modules)
    {
        var moduleProgresses = modules
            .Select(m => ModuleProgress.Create(m.ModuleId, m.LessonIds))
            .ToList();

        var totalLessons = moduleProgresses.Sum(m => m.TotalLessons);

        return new Progress
        {
            Id = ObjectId.GenerateNewId().ToString(),
            StudentId = studentId,
            CourseId = courseId,
            Status = ProgressStatus.NotStarted,
            CompletionPercentage = 0,
            TotalLessons = totalLessons,
            CompletedLessonsCount = 0,
            ModuleProgresses = moduleProgresses,
            StartedAt = DateTime.UtcNow,
            CompletedAt = null,
            LastUpdatedAt = DateTime.UtcNow
        };
    }

    public bool CompleteLesson(string moduleId, string lessonId)
    {
        var module = ModuleProgresses.FirstOrDefault(m => m.ModuleId == moduleId);
        if (module is null)
            throw new InvalidOperationException($"Module {moduleId} not found in progress.");

        var wasCompleted = module.CompleteLesson(lessonId);
        if (!wasCompleted)
            return false;

        CompletedLessonsCount++;
        CompletionPercentage = TotalLessons > 0
            ? Math.Round((double)CompletedLessonsCount / TotalLessons * 100, 2)
            : 0;

        Status = CompletedLessonsCount == TotalLessons
            ? ProgressStatus.Completed
            : ProgressStatus.InProgress;

        if (Status == ProgressStatus.Completed)
            CompletedAt = DateTime.UtcNow;

        LastUpdatedAt = DateTime.UtcNow;

        return true;
    }

    public bool IsFullyCompleted => Status == ProgressStatus.Completed;
}