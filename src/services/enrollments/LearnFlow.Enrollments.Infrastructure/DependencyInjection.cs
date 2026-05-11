using LearnFlow.Enrollments.Application.Common.Interfaces;
using LearnFlow.Enrollments.Infrastructure.ExternalServices;
using LearnFlow.Enrollments.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace LearnFlow.Enrollments.Infrastructure;

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

        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

        services.AddHttpClient<ICourseServiceClient, CourseServiceClient>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["CourseService:BaseUrl"]
                ?? throw new InvalidOperationException("CourseService:BaseUrl is not configured."));

            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "learnflow", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}