using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Progress.Domain.Entities;

public class CourseStructure
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }

    [BsonElement("courseId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CourseId { get; private set; }

    [BsonElement("modules")]
    public List<ModuleStructure> Modules { get; private set; }

    [BsonElement("lastUpdatedAt")]
    public DateTime LastUpdatedAt { get; private set; }

    private CourseStructure() { }

    public static CourseStructure Create(
        string courseId,
        List<ModuleStructure> modules)
    {
        return new CourseStructure
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CourseId = courseId,
            Modules = modules,
            LastUpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(List<ModuleStructure> modules)
    {
        Modules = modules;
        LastUpdatedAt = DateTime.UtcNow;
    }
}

public class ModuleStructure
{
    [BsonElement("moduleId")]
    public string ModuleId { get; set; }

    [BsonElement("lessonIds")]
    public List<string> LessonIds { get; set; }
}