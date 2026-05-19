using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Notifications.Domain.Entities;

public class Certificate
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }

    [BsonElement("certificateNumber")]
    public string CertificateNumber { get; private set; }

    [BsonElement("studentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string StudentId { get; private set; }

    [BsonElement("courseId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CourseId { get; private set; }

    [BsonElement("courseTitleSnapshot")]
    public string CourseTitleSnapshot { get; private set; }

    [BsonElement("completionPercentage")]
    public decimal CompletionPercentage { get; private set; }

    [BsonElement("IssuedAt")]
    public DateTime IssuedAt { get; private set; }

    public Certificate() { }

    public static Certificate Create(
    string studentId,
    string courseId,
    string courseTitleSnapshot,
    decimal completionPercentage)
    {
        return new Certificate
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CertificateNumber = GenerateCertificateNumber(),
            StudentId = studentId,
            CourseId = courseId,
            CourseTitleSnapshot = courseTitleSnapshot,
            CompletionPercentage = completionPercentage,
            IssuedAt = DateTime.UtcNow,
        };
    }

    private static string GenerateCertificateNumber()
    {
        var year = DateTime.UtcNow.Year;
        var random = Guid.NewGuid().ToString("N")[..8].ToUpper();
        return $"LF-{year}-{random}";
    }
}