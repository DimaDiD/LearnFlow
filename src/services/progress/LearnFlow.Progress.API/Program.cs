using FluentValidation;
using LearnFlow.Progress.API.Middleware;
using LearnFlow.Progress.Application.Common.Behaviors;
using LearnFlow.Progress.Application.Common.Mappings;
using LearnFlow.Progress.Application.Features.Progress.Commands.MarkLessonComplete;
using LearnFlow.Progress.Infrastructure;
using LearnFlow.Progress.Infrastructure.Persistence;
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
    Log.Information("Starting Progress Service...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("Service", "progress-service")
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Service} | {Message:lj}{NewLine}{Exception}")
        .WriteTo.Seq(context.Configuration["Seq:Url"] ?? "http://localhost:8081"));

    builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

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

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddValidatorsFromAssembly(typeof(MarkLessonCompleteCommand).Assembly);
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    builder.Services.AddAutoMapper(typeof(ProgressMappingProfile).Assembly);

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MarkLessonCompleteCommand).Assembly));

    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"]);
            diagnosticContext.Set("CorrelationId", httpContext.Response.Headers["X-Correlation-Id"].ToString());
        };
    });

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    using (var scope = app.Services.CreateScope())
    {
        var database = scope.ServiceProvider
            .GetRequiredService<MongoDB.Driver.IMongoDatabase>();
        await MongoDbInitializer.InitializeAsync(database);
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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
    Log.Fatal(ex, "Progress Service failed to start.");
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
            description = e.Value.Description,
            duration = e.Value.Duration.TotalMilliseconds,
            tags = e.Value.Tags
        })
    };

    return context.Response.WriteAsync(
        System.Text.Json.JsonSerializer.Serialize(response,
            new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            }));
}