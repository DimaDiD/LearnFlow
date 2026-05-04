using LearnFlow.Courses.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Courses.Domain.Entities;

public class Course
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }

    [BsonElement("title")]
    public string Title { get; private set; }

    [BsonElement("description")]
    public string Description { get; private set; }

    [BsonElement("instructorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string InstructorId { get; private set; }

    [BsonElement("category")]
    public string Category { get; private set; }

    [BsonElement("level")]
    [BsonRepresentation(BsonType.String)]
    public CourseLevel Level { get; private set; }

    [BsonElement("tags")]
    public List<string> Tags { get; private set; }

    [BsonElement("price")]
    public decimal Price { get; private set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public CourseStatus Status { get; private set; }

    [BsonElement("modules")]
    public List<Module> Modules { get; private set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

    [BsonElement("publishedAt")]
    public DateTime? PublishedAt { get; private set; }

    [BsonIgnore]
    public int TotalLessons => Modules.Sum(m => m.Lessons.Count);

    [BsonIgnore]
    public int TotalDurationMinutes => Modules.Sum(m => m.TotalDurationMinutes);

    private Course() { }

    public static Course Create(
        string title,
        string description,
        string instructorId,
        string category,
        CourseLevel level,
        List<string> tags,
        decimal price)
    {
        return new Course
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = title,
            Description = description,
            InstructorId = instructorId,
            Category = category,
            Level = level,
            Tags = tags,
            Price = price,
            Status = CourseStatus.Draft,
            Modules = new List<Module>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = null
        };
    }

    public void Update(
        string title,
        string description,
        string category,
        CourseLevel level,
        List<string> tags,
        decimal price)
    {
        Title = title;
        Description = description;
        Category = category;
        Level = level;
        Tags = tags;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddModule(Module module)
    {
        if (Status == CourseStatus.Archived)
            throw new InvalidOperationException("Cannot modify archived course.");

        if (Modules.Any(m => m.Order == module.Order))
            throw new InvalidOperationException(
                $"Module with order {module.Order} already exists.");

        Modules.Add(module);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveModule(string moduleId)
    {
        if (Status == CourseStatus.Archived)
            throw new InvalidOperationException("Cannot modify archived course.");

        var module = Modules.FirstOrDefault(m => m.Id == moduleId)
            ?? throw new InvalidOperationException($"Module {moduleId} not found.");

        Modules.Remove(module);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status == CourseStatus.Archived)
            throw new InvalidOperationException("Cannot publish archived course.");

        if (!Modules.Any())
            throw new InvalidOperationException("Cannot publish course without modules.");

        if (Modules.Any(m => !m.Lessons.Any()))
            throw new InvalidOperationException("All modules must have at least one lesson.");

        Status = CourseStatus.Published;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        if (Status == CourseStatus.Draft)
            throw new InvalidOperationException("Cannot archive draft course.");

        Status = CourseStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }
}