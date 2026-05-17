using LearnFlow.Search.Application.Common.Interfaces;
using LearnFlow.Search.Application.DTOs.Search;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Search.Application.Features.Search.Queries.SearchCourses;

public class SearchCoursesQueryHandler : IRequestHandler<SearchCoursesQuery, SearchResultDto>
{
    private readonly ISearchRepository _searchRepository;
    private readonly ILogger<SearchCoursesQueryHandler> _logger;

    public SearchCoursesQueryHandler(
        ISearchRepository searchRepository,
        ILogger<SearchCoursesQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<SearchResultDto> Handle(SearchCoursesQuery query, CancellationToken ct)
    {
        _logger.LogInformation(
            "Searching courses: query={Query} category={Category} level={Level} page={Page}",
            query.Query, query.Category, query.Level, query.Page);

        var (items, total) = await _searchRepository.SearchAsync(
            query.Query,
            query.Category,
            query.Level,
            query.Page,
            query.PageSize,
            query.SortBy,
            ct);

        var totalPages = (int)Math.Ceiling((double)total / query.PageSize);

        return new SearchResultDto(
            items.Select(c => new CourseSearchResultDto(
                c.Id,
                c.Title,
                c.Description,
                c.Category,
                c.Level,
                c.Tags,
                c.InstructorId,
                c.InstructorName,
                c.Price,
                c.EnrollmentCount,
                c.PublishedAt)).ToList(),
            total,
            query.Page,
            query.PageSize,
            totalPages);
    }
}