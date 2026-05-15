using LearnFlow.Progress.Application.Common.Interfaces;
using LearnFlow.Progress.Application.Features.Progress.Consumers;
using LearnFlow.Progress.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace LearnFlow.Progress.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MongoDB
        services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(configuration.GetConnectionString("MongoDB")));

        services.AddScoped<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>()
              .GetDatabase(configuration["MongoDB:DatabaseName"]
                ?? throw new InvalidOperationException("MongoDB:DatabaseName is not configured.")));

        // Repositories
        services.AddScoped<IProgressRepository, ProgressRepository>();
        services.AddScoped<ICourseStructureRepository, CourseStructureRepository>();

        // MassTransit + RabbitMQ
        services.AddMassTransit(x =>
        {
            // Register all consumers
            x.AddConsumer<CoursePublishedConsumer>();
            x.AddConsumer<CourseUpdatedConsumer>();
            x.AddConsumer<StudentEnrolledConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "learnflow", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });

                // Retry policy
                cfg.UseMessageRetry(r =>
                    r.Exponential(5,
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(5)));

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}