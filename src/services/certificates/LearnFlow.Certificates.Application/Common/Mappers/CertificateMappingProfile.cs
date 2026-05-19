using AutoMapper;
using LearnFlow.Certificates.Application.DTOs.Certificates;
using LearnFlow.Notifications.Domain.Entities;

namespace LearnFlow.Certificates.Application.Common.Mappers;

public class CertificateMappingProfile : Profile
{
    public CertificateMappingProfile()
    {
        CreateMap<Certificate, CertificateDto>();
    }
}