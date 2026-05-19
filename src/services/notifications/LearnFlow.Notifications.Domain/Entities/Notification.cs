using LearnFlow.Notifications.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Notifications.Domain.Entities;

public class Notification
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

    [BsonElement("recipientEmail")]
    public string RecipientEmail { get; private set; }

    [BsonElement("subject")]
    public string Subject { get; private set; }

    [BsonElement("notificationType")]
    [BsonRepresentation(BsonType.String)]
    public NotificationType NotificationType { get; private set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public NotificationStatus Status { get; private set; }

    [BsonElement("certificateNumber")]
    public string? CertificateNumber { get; private set; }

    [BsonElement("sentAt")]
    public DateTime SentAt { get; private set; }

    private Notification() { }

    public static Notification Create(
        string studentId,
        string courseId,
        string recipientEmail,
        string subject,
        NotificationType notificationType,
        string? certificateNumber = null)
    {
        return new Notification
        {
            Id = ObjectId.GenerateNewId().ToString(),
            StudentId = studentId,
            CourseId = courseId,
            RecipientEmail = recipientEmail,
            Subject = subject,
            NotificationType = notificationType,
            Status = NotificationStatus.Sent,
            CertificateNumber = certificateNumber,
            SentAt = DateTime.UtcNow
        };
    }

    public void MarkAsFailed()
    {
        Status = NotificationStatus.Failed;
    }
}