using LearnFlow.Courses.Application.DTOs.Courses;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Queries.GetInstructorCourses;

public record GetInstructorCoursesQuery(string InstructorId) : IRequest<List<CourseListItemDto>>;