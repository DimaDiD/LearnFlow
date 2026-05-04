using AutoMapper;
using LearnFlow.Courses.Application.Common.Interfaces;
using LearnFlow.Courses.Application.DTOs.Courses;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Queries.GetInstructorCourses;

public class GetInstructorCoursesQueryHandler
    : IRequestHandler<GetInstructorCoursesQuery, List<CourseListItemDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public GetInstructorCoursesQueryHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<List<CourseListItemDto>> Handle(
        GetInstructorCoursesQuery query,
        CancellationToken ct)
    {
        var courses = await _courseRepository.GetByInstructorIdAsync(query.InstructorId, ct);
        return _mapper.Map<List<CourseListItemDto>>(courses);
    }
}