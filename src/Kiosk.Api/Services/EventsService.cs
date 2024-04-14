using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Events;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class EventsService : IEventsService
{
    private readonly IEventsRepository _eventsRepository;
    private readonly IMapper _mapper;
    
    public EventsService(IEventsRepository eventsRepository, IMapper mapper)
    {
        _eventsRepository = eventsRepository;
        _mapper = mapper;
    }
    
    private GetEventResponse MapTranslatedEvent(Event events, Language language)
    {
        var mappedEvent = _mapper.Map<GetEventResponse>(events);
        mappedEvent.Language = language;

        switch (language)
        {
            case Language.Pl:
                mappedEvent.Name = events.Pl.Name;
                mappedEvent.Content = events.Pl.Content;
                break;

            case Language.En:
                mappedEvent.Name = events.En.Name;
                mappedEvent.Content = events.En.Content;
            
                break;
        }
        
        return mappedEvent;
    }
    
    public async Task<GetEventResponse?> GetTranslatedEvent(string eventId, Language language, CancellationToken cancellationToken)
    {
        var events = await _eventsRepository.GetEvent(eventId, cancellationToken);

        return events is not null ? MapTranslatedEvent(events, language) : null;
    }
    
    public async Task<IEnumerable<GetEventResponse>> GetTranslatedEvents(Language language, CancellationToken cancellationToken)
    {
        var eventsList = await _eventsRepository.GetManyEvents(cancellationToken);

        return eventsList.Select(events => MapTranslatedEvent(events, language));
    }
}