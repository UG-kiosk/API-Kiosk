using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Staff;

namespace KioskAPI.Mappers;

public class AcademicProfile : Profile
{
    public AcademicProfile()
    {
        CreateMap<Academic, AcademicResponse>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom((src, _, _, context) => src[(Language)context.Items["language"]]))
            .ForMember(dest => dest.Language, opt => opt.MapFrom((_, _, _, context) => (Language)context.Items["language"]));
    }
}

public class AcademicToListProfile : Profile
{
    public AcademicToListProfile() {
        CreateMap<Academic, AcademicToListResponse>()
            .ForMember(dest => dest.Units, opt => opt.MapFrom((src, _, _, context) => 
                src[(Language)context.Items["language"]].Posts.SelectMany(p => p.Faculty).Distinct().ToList()
            ))
            .ForMember(dest => dest.Language, opt => opt.MapFrom((_, _, _, context) => (Language)context.Items["language"]));
    }
}