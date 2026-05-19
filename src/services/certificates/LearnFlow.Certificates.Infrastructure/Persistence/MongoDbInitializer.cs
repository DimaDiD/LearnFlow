using LearnFlow.Notifications.Domain.Entities;
using MongoDB.Driver;

namespace LearnFlow.Certificates.Infrastructure.Persistence;

public static class MongoDbInitializer
{
    public static async Task InitializeAsync(IMongoDatabase database)
    {
        await CreateCertificateIndexesAsync(database);
    }

    private static async Task CreateCertificateIndexesAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<Certificate>("certificates");

        var uniqueIndex = Builders<Certificate>.IndexKeys
            .Ascending(c => c.StudentId)
            .Ascending(c => c.CourseId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Certificate>(
                uniqueIndex,
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "idx_certificates_studentId_courseId_unique"
                }));

        var studentIndex = Builders<Certificate>.IndexKeys.Ascending(c => c.StudentId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Certificate>(
                studentIndex,
                new CreateIndexOptions { Name = "idx_certificates_studentId" }));

        var certNumberIndex = Builders<Certificate>.IndexKeys.Ascending(c => c.CertificateNumber);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Certificate>(
                certNumberIndex,
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "idx_certificates_certificateNumber_unique"
                }));
    }
}