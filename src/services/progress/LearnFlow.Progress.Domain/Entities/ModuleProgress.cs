using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Progress.Domain.Entities;

public class ModuleProgress
{
    [BsonElement("moduleId")]
    public string ModuleId { get; private set; }

    [BsonElement("totalLessons")]
    public int TotalLessons { get; private set; }

    [BsonElement("completedLessonsCount")]
    public int CompletedLessonsCount { get; private set; }

    [BsonElement("lessonProgresses")]
    public List<LessonProgress> LessonProgresses { get; private set; }

    [BsonIgnore]
    public bool IsCompleted => CompletedLessonsCount == TotalLessons && TotalLessons > 0;

    private ModuleProgress() { }

    public static ModuleProgress Create(string moduleId, List<string> lessonIds)
    {
        return new ModuleProgress
        {
            ModuleId = moduleId,
            TotalLessons = lessonIds.Count,
            CompletedLessonsCount = 0,
            LessonProgresses = lessonIds.Select(LessonProgress.Create).ToList()
        };
    }

    public bool CompleteLesson(string lessonId)
    {
        var lesson = LessonProgresses.FirstOrDefault(l => l.LessonId == lessonId);
        if (lesson is null)
            return false;

        if (lesson.IsCompleted)
            return false;

        lesson.Complete();
        CompletedLessonsCount++;
        return true;
    }
}