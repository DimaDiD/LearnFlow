using LearnFlow.Progress.Application.DTOs.Progress;
using MediatR;

namespace LearnFlow.Progress.Application.Features.Progress.Queries.GetProgress;

public record GetProgressQuery(
    string StudentId,
    string CourseId
) : IRequest<ProgressDto>;