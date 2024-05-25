using AutoMapper;
using Kiosk.Abstractions.Models.LessonPlan;

namespace KioskAPI.Mappers;

public class LessonPlanProfile : Profile
{
    public LessonPlanProfile()
    {
        CreateMap<LessonPlan, GetLessonPlanResponse>();
    }
}

public class LessonPlanProfileSimplifiedContent : Profile
{
    public LessonPlanProfileSimplifiedContent()
    {
        CreateMap<LessonPlanDetails, LessonPlanContentSimplified>()
            .ForMember(dest => dest.Info, opt => opt.MapFrom(src => src.Info != null ? string.Join(";", src.Info) : null));

        CreateMap<CreateLessonPlanRequest, CreateLessonPlanSimplified>();
        
        CreateMap<LessonPlanContentSimplified, LessonPlanDetails>()
            .ForMember(dest => dest.Info, opt => opt.MapFrom(src => src.Info != null ? src.Info.Split(new[] { ';' }, System.StringSplitOptions.None).ToList() : null));
        
        CreateMap<CreateLessonPlanSimplified, CreateLessonPlanRequest>();
    }
}
