using System.Text.Json.Serialization;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.ArchiveCourse;

public record ArchiveCourseCommand(
    [property: JsonIgnore] string CourseId,
    string InstructorId
) : IRequest;