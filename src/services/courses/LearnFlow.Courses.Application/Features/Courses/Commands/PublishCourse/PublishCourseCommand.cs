using System.Text.Json.Serialization;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.PublishCourse;

public record PublishCourseCommand(
    [property: JsonIgnore] string CourseId,
    string InstructorId
) : IRequest;