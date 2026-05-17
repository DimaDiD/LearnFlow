namespace LearnFlow.Search.Application.DTOs.Search;

public record SearchResultDto(
    List<CourseSearchResultDto> Items,
    long Total,
    int Page,
    int PageSize,
    int TotalPages
);