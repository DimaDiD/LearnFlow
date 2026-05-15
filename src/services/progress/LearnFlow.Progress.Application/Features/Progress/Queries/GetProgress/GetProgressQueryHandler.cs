using AutoMapper;
using LearnFlow.Progress.Application.Common.Interfaces;
using LearnFlow.Progress.Application.DTOs.Progress;
using MediatR;

namespace LearnFlow.Progress.Application.Features.Progress.Queries.GetProgress;

public class GetProgressQueryHandler : IRequestHandler<GetProgressQuery, ProgressDto>
{
    private readonly IProgressRepository _progressRepository;
    private readonly IMapper _mapper;

    public GetProgressQueryHandler(
        IProgressRepository progressRepository,
        IMapper mapper)
    {
        _progressRepository = progressRepository;
        _mapper = mapper;
    }

    public async Task<ProgressDto> Handle(GetProgressQuery query, CancellationToken ct)
    {
        var progress = await _progressRepository
            .GetByStudentAndCourseAsync(query.StudentId, query.CourseId, ct)
            ?? throw new InvalidOperationException(
                $"Progress not found for student {query.StudentId} course {query.CourseId}.");

        return _mapper.Map<ProgressDto>(progress);
    }
}