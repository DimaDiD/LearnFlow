using LearnFlow.Notifications.Infrastructure;
using LearnFlow.Notifications.Infrastructure.Persistence;
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
    Log.Information("Starting Notifications Service...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("Service", "notifications-service")
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Service} | {Message:lj}{NewLine}{Exception}")
        .WriteTo.Seq(context.Configuration["Seq:Url"] ?? "http://localhost:8081"));

    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
        .AddMongoDb(
            mongodbConnectionString: builder.Configuration.GetConnectionString("MongoDB")!,
            name: "mongodb",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "ready", "db" },
            timeout: TimeSpan.FromSeconds(3))
        .AddRabbitMQ(
            rabbitConnectionString: $"amqp://{builder.Configuration["RabbitMQ:Username"]}:{builder.Configuration["RabbitMQ:Password"]}@localhost:5672/learnflow",
            name: "rabbitmq",
            failureStatus: HealthStatus.Degraded,
            tags: new[] { "ready", "messaging" },
            timeout: TimeSpan.FromSeconds(3));

    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var database = scope.ServiceProvider
            .GetRequiredService<MongoDB.Driver.IMongoDatabase>();
        await MongoDbInitializer.InitializeAsync(database);
    }

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
    Log.Fatal(ex, "Notifications Service failed to start.");
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