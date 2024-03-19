using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Staff;

namespace KioskAPI.Mappers;

public class AcademicProfile : Profile
{
    public AcademicProfile()
    {
        CreateMap<Academic, AcademicResponse>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom((src, dest, destMember, context) => src[(Language)context.Items["language"]]))
            .ForMember(dest => dest.Language, opt => opt.MapFrom((src, dest, destMember, context) => (Language)context.Items["language"]));
    }
}