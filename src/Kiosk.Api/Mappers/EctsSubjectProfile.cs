using AutoMapper;
using Kiosk.Abstractions.Models;

namespace KioskAPI.Mappers;

public class EctsProfile : Profile
{
    public EctsProfile()
    {
        CreateMap<EctsSubjectDocument, SubjectResponse>();
    }
}