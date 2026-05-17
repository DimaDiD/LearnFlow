using LearnFlow.Search.Application.DTOs.Search;
using MediatR;

namespace LearnFlow.Search.Application.Features.Search.Queries.SearchCourses;

public record SearchCoursesQuery(
    string? Query,
    string? Category,
    string? Level,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null
) : IRequest<SearchResultDto>;