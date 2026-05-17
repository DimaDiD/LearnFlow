using FluentValidation;
using LearnFlow.Search.API.Middleware;
using LearnFlow.Search.Application.Common.Behaviors;
using LearnFlow.Search.Application.Features.Search.Queries.SearchCourses;
using LearnFlow.Search.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Search Service...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("Service", "search-service")
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Service} | {Message:lj}{NewLine}{Exception}")
        .WriteTo.Seq(context.Configuration["Seq:Url"] ?? "http://localhost:8081"));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
        .AddRabbitMQ(
            rabbitConnectionString: $"amqp://{builder.Configuration["RabbitMQ:Username"]}:{builder.Configuration["RabbitMQ:Password"]}@localhost:5672/learnflow",
            name: "rabbitmq",
            failureStatus: HealthStatus.Degraded,
            tags: new[] { "ready", "messaging" },
            timeout: TimeSpan.FromSeconds(3))
        .AddElasticsearch(
            builder.Configuration["Elasticsearch:Url"]!,
            name: "elasticsearch",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "ready", "search" },
            timeout: TimeSpan.FromSeconds(3)
        );

    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(SearchCoursesQuery).Assembly));

    builder.Services.AddValidatorsFromAssembly(typeof(SearchCoursesQuery).Assembly);
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var elasticsearchClient = scope.ServiceProvider
            .GetRequiredService<Elastic.Clients.Elasticsearch.ElasticsearchClient>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILogger<Program>>();
        await LearnFlow.Search.Infrastructure.Elasticsearch.ElasticsearchIndexInitializer
            .InitializeAsync(elasticsearchClient, logger);
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("CorrelationId",
                httpContext.Response.Headers["X-Correlation-Id"].ToString());
        };
    });

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.MapControllers();

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live"),
        ResponseWriter = WriteHealthResponse
    });
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = WriteHealthResponse
    });
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = WriteHealthResponse
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Search Service failed to start.");
}
finally
{
    Log.CloseAndFlush();
}

static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";
    var response = new
    {
        status = report.Status.ToString(),
        duration = report.TotalDuration.TotalMilliseconds,
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            duration = e.Value.Duration.TotalMilliseconds,
            tags = e.Value.Tags
        })
    };
    return context.Response.WriteAsync(
        System.Text.Json.JsonSerializer.Serialize(response,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
}