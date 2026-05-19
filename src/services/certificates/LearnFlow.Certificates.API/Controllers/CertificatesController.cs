using LearnFlow.Certificates.Application.Features.Certificates.Queries.GetCertificateById;
using LearnFlow.Notifications.Application.Features.Certificates.Queries.GetMyCertificates;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearnFlow.Certificates.API.Controllers;

[ApiController]
[Route("api/certificates")]
public class CertificatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CertificatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("my/{studentId}")]
    public async Task<IActionResult> GetStudentCertificates(
        string studentId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetMyCertificatesQuery(studentId, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{certificateId}")]
    public async Task<IActionResult> GetCertificateById(
        string certificateId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCertificateByIdQuery(certificateId), ct);
        return Ok(result);
    }
}