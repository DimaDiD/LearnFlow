using LearnFlow.Search.Application.Features.Search.Queries.SearchCourses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearnFlow.Search.API.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] SearchCoursesQuery request,
        CancellationToken ct = default)
    {
        var searchResult = await _mediator.Send(request, ct);
        return Ok(searchResult);
    }
}

