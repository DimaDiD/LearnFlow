using AutoMapper;
using LearnFlow.Courses.Application.DTOs.Courses;
using LearnFlow.Courses.Domain.Entities;

namespace LearnFlow.Courses.Application.Common.Mappings;

public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        CreateMap<Lesson, LessonDto>();
        CreateMap<Module, ModuleDto>();
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.TotalLessons,
                opt => opt.MapFrom(src => src.TotalLessons))
            .ForMember(dest => dest.TotalDurationMinutes,
                opt => opt.MapFrom(src => src.TotalDurationMinutes));
        CreateMap<Course, CourseListItemDto>()
            .ForMember(dest => dest.TotalLessons,
                opt => opt.MapFrom(src => src.TotalLessons))
            .ForMember(dest => dest.TotalDurationMinutes,
                opt => opt.MapFrom(src => src.TotalDurationMinutes));
    }
}