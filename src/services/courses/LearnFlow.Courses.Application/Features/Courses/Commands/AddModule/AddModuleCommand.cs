using System.Text.Json.Serialization;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.AddModule;

public record AddModuleCommand(
    string CourseId,
    string InstructorId,
    string Title,
    string Description,
    int Order
) : IRequest<string>;