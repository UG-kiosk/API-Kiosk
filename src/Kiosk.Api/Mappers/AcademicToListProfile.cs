using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Staff;

namespace KioskAPI.Mappers;

public class AcademicToListProfile : Profile
{
   public AcademicToListProfile() {
        CreateMap<Academic, AcademicToListResponse>()
            .ForMember(dest => dest.Units, opt => opt.MapFrom((src, dest, destMember, context) => 
                    src[(Language)context.Items["language"]].Posts.SelectMany(p => p.Faculty).Distinct().ToList()
            ))
            .ForMember(dest => dest.Language, opt => opt.MapFrom((src, dest, destMember, context) => (Language)context.Items["language"]));
    }
}