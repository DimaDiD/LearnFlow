using FluentValidation;
using LearnFlow.Courses.API.Middleware;
using LearnFlow.Courses.Application.Common.Behaviors;
using LearnFlow.Courses.Application.Common.Mappings;
using LearnFlow.Courses.Application.Features.Courses.Commands.CreateCourse;
using LearnFlow.Courses.Infrastructure;
using LearnFlow.Courses.Infrastructure.Persistence;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateCourseCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CreateCourseCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddAutoMapper(typeof(CourseMappingProfile).Assembly);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();