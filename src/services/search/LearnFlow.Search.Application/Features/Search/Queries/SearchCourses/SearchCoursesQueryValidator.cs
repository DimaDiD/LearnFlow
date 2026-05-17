using FluentValidation;

namespace LearnFlow.Search.Application.Features.Search.Queries.SearchCourses;

public class SearchCoursesQueryValidator : AbstractValidator<SearchCoursesQuery>
{
    public SearchCoursesQueryValidator()
    {
        RuleFor(x => x.Page).NotEmpty().GreaterThan(0);
        RuleFor(x => x.PageSize).NotEmpty().GreaterThan(0);
    }
}