using LearnFlow.Notifications.Application.Common.Interfaces;
using LearnFlow.Notifications.Domain.Entities;
using LearnFlow.Notifications.Domain.Enums;
using MongoDB.Driver;

namespace LearnFlow.Notifications.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _notifications;

    public NotificationRepository(IMongoDatabase database)
    {
        _notifications = database.GetCollection<Notification>("notifications");
    }

    public Task<Notification?> GetByStudentCourseAndTypeAsync(
        string studentId,
        string courseId,
        NotificationType type,
        CancellationToken ct = default)
        => _notifications
            .Find(n => n.StudentId == studentId
                && n.CourseId == courseId
                && n.NotificationType == type)
            .FirstOrDefaultAsync(ct);

    public Task InsertAsync(Notification notification, CancellationToken ct = default)
        => _notifications.InsertOneAsync(notification, cancellationToken: ct);
}