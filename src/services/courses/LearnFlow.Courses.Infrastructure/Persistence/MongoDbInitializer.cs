using LearnFlow.Courses.Domain.Entities;
using MongoDB.Driver;

namespace LearnFlow.Courses.Infrastructure.Persistence;

public static class MongoDbInitializer
{
    public static async Task InitializeAsync(IMongoDatabase database)
    {
        await CreateCourseIndexesAsync(database);
    }

    private static async Task CreateCourseIndexesAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<Course>("courses");

        var statusIndex = Builders<Course>.IndexKeys.Ascending(c => c.Status);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Course>(
                statusIndex,
                new CreateIndexOptions { Name = "idx_courses_status" }));

        var instructorIndex = Builders<Course>.IndexKeys.Ascending(c => c.InstructorId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Course>(
                instructorIndex,
                new CreateIndexOptions { Name = "idx_courses_instructorId" }));
    }
}