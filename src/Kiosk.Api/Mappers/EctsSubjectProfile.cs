using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;

namespace KioskAPI.Mappers;

public class EctsProfile : Profile
{
    public EctsProfile()
    {
        CreateMap<EctsSubjectDocument, SubjectResponse>()
            .ForMember(dest=> dest.Subject, opt => opt.MapFrom((document,_,_,context)=> document[(Language)context.Items["language"]].Subject ))
            .ForMember(dest=> dest.Major, opt => opt.MapFrom((document,_,_,context)=> document[(Language)context.Items["language"]].Major ))
            .ForMember(dest=> dest.Speciality, opt => opt.MapFrom((document,_,_,context)=> document[(Language)context.Items["language"]].Speciality ));
        
        CreateMap<EctsSubjectDocument, EctsSubjectCreateRequest>()
            .ForMember(dest=> dest.Subject, opt => opt.MapFrom((document,_,_,context)=> document[(Language)context.Items["language"]].Subject ))
            .ForMember(dest=> dest.Major, opt => opt.MapFrom((document,_,_,context)=> document[(Language)context.Items["language"]].Major ))
            .ForMember(dest=> dest.Speciality, opt => opt.MapFrom((document,_,_,context)=> document[(Language)context.Items["language"]].Speciality ));
    }
}