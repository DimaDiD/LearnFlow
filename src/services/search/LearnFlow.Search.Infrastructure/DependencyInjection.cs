using Elastic.Clients.Elasticsearch;
using LearnFlow.Search.Application.Common.Interfaces;
using LearnFlow.Search.Application.Features.Search.Consumers;
using LearnFlow.Search.Infrastructure.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LearnFlow.Search.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var elasticsearchUrl = configuration["Elasticsearch:Url"]
            ?? throw new InvalidOperationException("Elasticsearch:Url is not configured.");

        var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
            .DefaultIndex("courses");

        services.AddSingleton(new ElasticsearchClient(settings));
        services.AddScoped<ISearchRepository, SearchRepository>();
     
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserRegisteredConsumer>();
            x.AddConsumer<CoursePublishedConsumer>();
            x.AddConsumer<CourseUpdatedConsumer>();
            x.AddConsumer<CourseArchivedConsumer>();

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