using AutoMapper;
using Kiosk.Abstractions.Dtos;
using Kiosk.Abstractions.Models;

namespace KioskAPI.Mappers;

public class MajorsProfile : Profile
{
    public MajorsProfile()
    {
        CreateMap<Major, MajorOutputDto>();
    }
}
