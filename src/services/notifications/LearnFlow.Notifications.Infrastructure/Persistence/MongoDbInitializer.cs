using LearnFlow.Notifications.Domain.Entities;
using MongoDB.Driver;

namespace LearnFlow.Notifications.Infrastructure.Persistence;

public static class MongoDbInitializer
{
    public static async Task InitializeAsync(IMongoDatabase database)
    {
        await CreateNotificationIndexesAsync(database);
    }

    private static async Task CreateNotificationIndexesAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<Notification>("notifications");

        var uniqueIndex = Builders<Notification>.IndexKeys
            .Ascending(n => n.StudentId)
            .Ascending(n => n.CourseId)
            .Ascending(n => n.NotificationType);

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Notification>(
                uniqueIndex,
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "idx_notifications_studentId_courseId_type_unique"
                }));
    }
}