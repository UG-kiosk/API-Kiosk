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