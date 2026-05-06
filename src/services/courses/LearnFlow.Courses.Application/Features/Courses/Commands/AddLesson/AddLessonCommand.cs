using System.Text.Json.Serialization;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.AddLesson;

public record AddLessonCommand(
    string CourseId,
    string ModuleId,
    string InstructorId,
    string Title,
    string Description,
    string VideoUrl,
    int DurationMinutes,
    int Order
) : IRequest<string>;