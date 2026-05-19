using LearnFlow.Notifications.Application.Common.Interfaces;
using LearnFlow.Notifications.Application.Features.Notifications.Consumers;
using LearnFlow.Notifications.Infrastructure.Email;
using LearnFlow.Notifications.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace LearnFlow.Notifications.Infrastructure;

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

        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<IEmailService, EmailService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<StudentEnrolledConsumer>();
            x.AddConsumer<CertificateIssuedConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "learnflow", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });

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