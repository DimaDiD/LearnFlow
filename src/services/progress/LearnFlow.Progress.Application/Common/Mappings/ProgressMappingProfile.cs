using AutoMapper;
using LearnFlow.Progress.Application.DTOs.Progress;
using LearnFlow.Progress.Domain.Entities;
using ProgressEntity = LearnFlow.Progress.Domain.Entities.Progress;

namespace LearnFlow.Progress.Application.Common.Mappings;

public class ProgressMappingProfile : Profile
{
    public ProgressMappingProfile()
    {
        CreateMap<LessonProgress, LessonProgressDto>();

        CreateMap<ModuleProgress, ModuleProgressDto>()
            .ForMember(dest => dest.IsCompleted,
                opt => opt.MapFrom(src => src.IsCompleted));

        CreateMap<ProgressEntity, ProgressDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));
    }
}