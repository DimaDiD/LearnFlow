using AutoMapper;
using LearnFlow.Certificates.Application.Common.Interfaces;
using LearnFlow.Certificates.Application.DTOs.Certificates;
using MediatR;

namespace LearnFlow.Notifications.Application.Features.Certificates.Queries.GetMyCertificates;

public class GetMyCertificatesQueryHandler
    : IRequestHandler<GetMyCertificatesQuery, List<CertificateDto>>
{
    private readonly ICertificateRepository _certificateRepository;
    private readonly IMapper _mapper;

    public GetMyCertificatesQueryHandler(ICertificateRepository certificateRepository, IMapper mapper)
    {
        _certificateRepository = certificateRepository;
        _mapper = mapper;
    }

    public async Task<List<CertificateDto>> Handle(
        GetMyCertificatesQuery query,
        CancellationToken ct)
    {
        var certificates = await _certificateRepository.GetByStudentIdAsync(query.StudentId, query.Page, query.PageSize, ct);

        if (!certificates.Any())
            throw new InvalidOperationException("You doesn't have any certificates!");

        return _mapper.Map<List<CertificateDto>>(certificates);
    }
}