using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Courses.Domain.Entities;

public class Lesson
{
    [BsonElement("id")]
    public string Id { get; private set; }

    [BsonElement("title")]
    public string Title { get; private set; }

    [BsonElement("description")]
    public string Description { get; private set; }

    [BsonElement("videoUrl")]
    public string VideoUrl { get; private set; }

    [BsonElement("durationMinutes")]
    public int DurationMinutes { get; private set; }

    [BsonElement("order")]
    public int Order { get; private set; }

    private Lesson() { }

    public static Lesson Create(
        string title,
        string description,
        string videoUrl,
        int durationMinutes,
        int order)
    {
        return new Lesson
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = title,
            Description = description,
            VideoUrl = videoUrl,
            DurationMinutes = durationMinutes,
            Order = order
        };
    }

    public void Update(string title, string description, string videoUrl, int durationMinutes)
    {
        Title = title;
        Description = description;
        VideoUrl = videoUrl;
        DurationMinutes = durationMinutes;
    }
}