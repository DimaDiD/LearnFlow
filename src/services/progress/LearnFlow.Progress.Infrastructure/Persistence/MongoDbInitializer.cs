using LearnFlow.Progress.Domain.Entities;
using MongoDB.Driver;
using ProgressEntity = LearnFlow.Progress.Domain.Entities.Progress;

namespace LearnFlow.Progress.Infrastructure.Persistence;

public static class MongoDbInitializer
{
    public static async Task InitializeAsync(IMongoDatabase database)
    {
        await CreateProgressIndexesAsync(database);
        await CreateCourseStructureIndexesAsync(database);
    }

    private static async Task CreateProgressIndexesAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<ProgressEntity>("progress");

        var uniqueIndex = Builders<ProgressEntity>.IndexKeys
            .Ascending(p => p.StudentId)
            .Ascending(p => p.CourseId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<ProgressEntity>(
                uniqueIndex,
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "idx_progress_studentId_courseId_unique"
                }));

        var studentIndex = Builders<ProgressEntity>.IndexKeys
            .Ascending(p => p.StudentId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<ProgressEntity>(
                studentIndex,
                new CreateIndexOptions { Name = "idx_progress_studentId" }));

        var statusIndex = Builders<ProgressEntity>.IndexKeys
            .Ascending(p => p.Status);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<ProgressEntity>(
                statusIndex,
                new CreateIndexOptions { Name = "idx_progress_status" }));
    }

    private static async Task CreateCourseStructureIndexesAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<CourseStructure>("course_structures");

        var courseIdIndex = Builders<CourseStructure>.IndexKeys
            .Ascending(s => s.CourseId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<CourseStructure>(
                courseIdIndex,
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "idx_course_structures_courseId_unique"
                }));
    }
}