using AutoMapper;
using LearnFlow.Certificates.Application.Common.Interfaces;
using LearnFlow.Certificates.Application.DTOs.Certificates;
using MediatR;

namespace LearnFlow.Certificates.Application.Features.Certificates.Queries.GetCertificateById;

public class GetCertificateByIdQueryHandler : IRequestHandler<GetCertificateByIdQuery, CertificateDto?>
{
    private readonly ICertificateRepository _certificateRepository;
    private readonly IMapper _mapper;

    public GetCertificateByIdQueryHandler(ICertificateRepository certificateRepository, IMapper mapper)
    {
        _certificateRepository = certificateRepository;
        _mapper = mapper;
    }

    public async Task<CertificateDto?> Handle(
        GetCertificateByIdQuery query,
        CancellationToken ct)
    {
        var certificate = await _certificateRepository.GetByIdAsync(query.CertificateId, ct);

        if (certificate is null)
            return null;
        return _mapper.Map<CertificateDto>(certificate);
    }
}