using LearnFlow.Notifications.Domain.Entities;
using LearnFlow.Notifications.Domain.Enums;

namespace LearnFlow.Notifications.Application.Common.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByStudentCourseAndTypeAsync(
        string studentId,
        string courseId,
        NotificationType type,
        CancellationToken ct = default);
    Task InsertAsync(Notification notification, CancellationToken ct = default);
}