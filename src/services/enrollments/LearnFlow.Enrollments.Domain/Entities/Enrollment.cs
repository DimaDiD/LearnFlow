using LearnFlow.Enrollments.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Enrollments.Domain.Entities;

public class Enrollment
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

    [BsonElement("instructorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string InstructorId { get; private set; }

    [BsonElement("courseTitleSnapshot")]
    public string CourseTitleSnapshot { get; private set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public EnrollmentStatus Status { get; private set; }

    [BsonElement("enrolledAt")]
    public DateTime EnrolledAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

    private Enrollment() { }

    public static Enrollment Create(
        string studentId,
        string courseId,
        string instructorId,
        string courseTitleSnapshot)
    {
        return new Enrollment
        {
            Id = ObjectId.GenerateNewId().ToString(),
            StudentId = studentId,
            CourseId = courseId,
            InstructorId = instructorId,
            CourseTitleSnapshot = courseTitleSnapshot,
            Status = EnrollmentStatus.Active,
            EnrolledAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Cancel()
    {
        if (Status == EnrollmentStatus.Cancelled)
            throw new InvalidOperationException("Enrollment is already cancelled.");

        Status = EnrollmentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        if (Status == EnrollmentStatus.Active)
            throw new InvalidOperationException("Enrollment is already active.");

        Status = EnrollmentStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }
}