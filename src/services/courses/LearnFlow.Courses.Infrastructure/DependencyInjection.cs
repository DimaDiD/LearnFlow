using LearnFlow.Courses.Application.Common.Interfaces;
using LearnFlow.Courses.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace LearnFlow.Courses.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(configuration.GetConnectionString("MongoDB")));

        services.AddScoped<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>()
              .GetDatabase(configuration["MongoDB:DatabaseName"]
                ?? throw new InvalidOperationException("MongoDB:DatabaseName is not configured.")));

        services.AddScoped<ICourseRepository, CourseRepository>();

        return services;
    }
}