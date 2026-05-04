using LearnFlow.Courses.Application.DTOs.Courses;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Queries.GetCourseById;

public record GetCourseByIdQuery(string CourseId) : IRequest<CourseDto>;