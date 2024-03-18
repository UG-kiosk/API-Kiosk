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
    
    private EventResponse MapTranslatedEvent(Event events, Language language)
    {
        var mappedEvent = _mapper.Map<EventResponse>(events);
        mappedEvent.Language = language;

        switch (language)
        {
            case Language.Pl:
                mappedEvent.Name = events.Pl.name;
                mappedEvent.Content = events.Pl.content;
                break;
            
            // Language.En will be  implemented
            
            // case Language.En:
            //     mappedEvent.Name = events.En.Name;
            //     mappedEvent.Content = events.En.Content;
            //
            //     break;
        }
        
        return mappedEvent;
    }
    
    public async Task<EventResponse?> GetTranslatedEvent(string eventId, Language language, CancellationToken cancellationToken)
    {
        var events = await _eventsRepository.GetEvent(eventId, cancellationToken);

        return events != null ? MapTranslatedEvent(events, language) : null;
    }
    
    public async Task<IEnumerable<EventResponse>> GetTranslatedEvents(Source? source, Language language, CancellationToken cancellationToken)
    {
        var eventsList = await _eventsRepository.GetManyEvents(source, cancellationToken);

        return eventsList.Select(events => MapTranslatedEvent(events, language));
    }
    



}