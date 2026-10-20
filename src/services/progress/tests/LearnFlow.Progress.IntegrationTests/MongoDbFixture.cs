using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace LearnFlow.Progress.IntegrationTests;

public class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .Build();

    public IMongoDatabase Database { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var client = new MongoClient(_container.GetConnectionString());
        Database = client.GetDatabase("learnflow_progress_test");
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}