using LearnFlow.Certificates.Application.DTOs.Certificates;
using MediatR;

namespace LearnFlow.Certificates.Application.Features.Certificates.Queries.GetCertificateById;

public record GetCertificateByIdQuery(
    string CertificateId
) : IRequest<CertificateDto?>;