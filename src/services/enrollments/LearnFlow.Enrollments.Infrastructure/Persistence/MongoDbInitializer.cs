using LearnFlow.Enrollments.Domain.Entities;
using MongoDB.Driver;

namespace LearnFlow.Enrollments.Infrastructure.Persistence;

public static class MongoDbInitializer
{
    public static async Task InitializeAsync(IMongoDatabase database)
    {
        await CreateEnrollmentIndexesAsync(database);
    }

    private static async Task CreateEnrollmentIndexesAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<Enrollment>("enrollments");

        var uniqueIndex = Builders<Enrollment>.IndexKeys
            .Ascending(e => e.StudentId)
            .Ascending(e => e.CourseId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Enrollment>(
                uniqueIndex,
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "idx_enrollments_studentId_courseId_unique"
                }));

        var studentIndex = Builders<Enrollment>.IndexKeys.Ascending(e => e.StudentId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Enrollment>(
                studentIndex,
                new CreateIndexOptions { Name = "idx_enrollments_studentId" }));

        var courseIndex = Builders<Enrollment>.IndexKeys.Ascending(e => e.CourseId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Enrollment>(
                courseIndex,
                new CreateIndexOptions { Name = "idx_enrollments_courseId" }));

        var instructorIndex = Builders<Enrollment>.IndexKeys.Ascending(e => e.InstructorId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Enrollment>(
                instructorIndex,
                new CreateIndexOptions { Name = "idx_enrollments_instructorId" }));

        var statusIndex = Builders<Enrollment>.IndexKeys.Ascending(e => e.Status);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Enrollment>(
                statusIndex,
                new CreateIndexOptions { Name = "idx_enrollments_status" }));
    }
}