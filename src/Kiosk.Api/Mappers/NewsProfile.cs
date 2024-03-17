using AutoMapper;
using Kiosk.Abstractions.Models.News;

namespace KioskAPI.Mappers;

public class NewsProfile : Profile
{
    public NewsProfile()
    {
        CreateMap<News, NewsResponse>();
    }
}