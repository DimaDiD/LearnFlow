using AutoMapper;
using LearnFlow.Courses.Application.Common.Interfaces;
using LearnFlow.Courses.Application.DTOs.Courses;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Queries.GetCourseById;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public GetCourseByIdQueryHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<CourseDto> Handle(GetCourseByIdQuery query, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(query.CourseId, ct)
            ?? throw new InvalidOperationException($"Course {query.CourseId} not found.");

        return _mapper.Map<CourseDto>(course);
    }
}