using AutoMapper;
using LearnFlow.Courses.Application.Common.Interfaces;
using LearnFlow.Courses.Application.DTOs.Courses;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Queries.GetCourses;

public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, GetCoursesResult>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public GetCoursesQueryHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<GetCoursesResult> Handle(GetCoursesQuery query, CancellationToken ct)
    {
        var courses = await _courseRepository.GetPublishedAsync(query.Page, query.PageSize, ct);
        var totalCount = await _courseRepository.CountPublishedAsync(ct);

        return new GetCoursesResult(
            _mapper.Map<List<CourseListItemDto>>(courses),
            (int)totalCount,
            query.Page,
            query.PageSize);
    }
}