using LearnFlow.Certificates.Application.DTOs.Certificates;
using MediatR;

namespace LearnFlow.Notifications.Application.Features.Certificates.Queries.GetMyCertificates;
public record GetMyCertificatesQuery(string StudentId, int Page = 1, int PageSize = 10) : IRequest<List<CertificateDto>>;
