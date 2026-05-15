using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Progress.Domain.Entities;

public class LessonProgress
{
    [BsonElement("lessonId")]
    public string LessonId { get; private set; }

    [BsonElement("isCompleted")]
    public bool IsCompleted { get; private set; }

    [BsonElement("completedAt")]
    public DateTime? CompletedAt { get; private set; }

    private LessonProgress() { }

    public static LessonProgress Create(string lessonId)
    {
        return new LessonProgress
        {
            LessonId = lessonId,
            IsCompleted = false,
            CompletedAt = null
        };
    }

    public void Complete()
    {
        if (IsCompleted)
            return; 

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
    }
}