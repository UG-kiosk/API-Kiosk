using AutoMapper;
using Kiosk.Abstractions.Models.Events;

namespace KioskAPI.Mappers;

public class EventsProfile : Profile
{
    public EventsProfile()
    {
        CreateMap<Event, GetEventResponse>();
    }
}