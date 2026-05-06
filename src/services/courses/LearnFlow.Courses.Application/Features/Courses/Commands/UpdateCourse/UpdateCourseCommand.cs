using System.Text.Json.Serialization;
using LearnFlow.Courses.Domain.Enums;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.UpdateCourse;

public record UpdateCourseCommand(
    string CourseId,
    string InstructorId,
    string Title,
    string Description,
    string Category,
    CourseLevel Level,
    List<string> Tags,
    decimal Price
) : IRequest;