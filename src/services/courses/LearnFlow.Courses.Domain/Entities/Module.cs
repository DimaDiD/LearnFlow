using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Courses.Domain.Entities;

public class Module
{
    [BsonElement("id")]
    public string Id { get; private set; }

    [BsonElement("title")]
    public string Title { get; private set; }

    [BsonElement("description")]
    public string Description { get; private set; }

    [BsonElement("order")]
    public int Order { get; private set; }

    [BsonElement("lessons")]
    public List<Lesson> Lessons { get; private set; }

    private Module() { }

    public static Module Create(string title, string description, int order)
    {
        return new Module
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = title,
            Description = description,
            Order = order,
            Lessons = new List<Lesson>()
        };
    }

    public void Update(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public void AddLesson(Lesson lesson)
    {
        if (Lessons.Any(l => l.Order == lesson.Order))
            throw new InvalidOperationException(
                $"Lesson with order {lesson.Order} already exists in this module.");

        Lessons.Add(lesson);
    }

    public void RemoveLesson(string lessonId)
    {
        var lesson = Lessons.FirstOrDefault(l => l.Id == lessonId)
            ?? throw new InvalidOperationException($"Lesson {lessonId} not found.");

        Lessons.Remove(lesson);
    }

    public int TotalDurationMinutes => Lessons.Sum(l => l.DurationMinutes);
}