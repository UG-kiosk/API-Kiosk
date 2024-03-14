using AutoMapper;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Major;

namespace KioskAPI.Mappers;

public class MajorsProfile : Profile
{
    public MajorsProfile()
    {
        CreateMap<Major, MajorResponse>();
    }
}
