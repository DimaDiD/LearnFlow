using LearnFlow.Courses.Application.DTOs.Courses;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Queries.GetCourses;

public record GetCoursesQuery(
    int Page = 1,
    int PageSize = 10
) : IRequest<GetCoursesResult>;

public record GetCoursesResult(
    List<CourseListItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);