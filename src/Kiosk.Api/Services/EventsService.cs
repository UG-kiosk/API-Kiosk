using System.Collections.Immutable;
using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Events;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.Translation;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class EventsService : IEventsService
{
    private readonly IEventsRepository _eventsRepository;
    private readonly IMapper _mapper;
    private readonly ITranslatorService _translatorService;
    
    public EventsService(IEventsRepository eventsRepository, IMapper mapper, ITranslatorService translatorService)
    {
        _eventsRepository = eventsRepository;
        _mapper = mapper;
        _translatorService = translatorService;
    }
    
    private EventResponse MapTranslatedEvent(Event events, Language language)
    {
        var mappedEvent = _mapper.Map<EventResponse>(events);
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
    
    public async Task<EventResponse?> GetTranslatedEvent(string eventId, Language language, CancellationToken cancellationToken)
    {
        var events = await _eventsRepository.GetEvent(eventId, cancellationToken);

        return events is not null ? MapTranslatedEvent(events, language) : null;
    }
    
    public async Task<(IEnumerable<EventResponse>?, Pagination Pagination)> GetTranslatedEvents(Language language, int? page, int? itemsPerPage, CancellationToken cancellationToken)
    {
        var pagination = new Pagination
        {
            Page = page.GetValueOrDefault(1),
            ItemsPerPage = itemsPerPage.GetValueOrDefault(5)
        };
        
        var (eventsList, updatedPagination) = await _eventsRepository.GetManyEvents(pagination, cancellationToken);

        return (eventsList.Select(events => MapTranslatedEvent(events, language)), updatedPagination);
    }

    public async Task CreateEvent(IEnumerable<CreateEventRequest> createEventRequests,
        CancellationToken cancellationToken)
    {
        var mappedEvents = await TranslateEvents(createEventRequests, cancellationToken);
        await _eventsRepository.CreateEvent(mappedEvents, cancellationToken);
    }
    
    private async Task<IEnumerable<Event>> TranslateEvents(
        IEnumerable<CreateEventRequest> createEventRequests,
        CancellationToken cancellationToken)
    {
        var groupedByLanguage = createEventRequests.GroupBy(request => request.Language);
        ImmutableList<Language> supportedLanguages = new List<Language> { Language.En, Language.Pl }.ToImmutableList();

        var translationTasks = groupedByLanguage.Select(async eventLanguageGroup =>
        {
            var translationContent = eventLanguageGroup.Select(eventGroup => new TranslationRequest<EventDetails>
            {
                UniqueKey = Guid.NewGuid().ToString(),
                TranslationPayload = eventGroup.EventDetails
            }).ToList();

            var translationTask = await _translatorService.Translate(
                translationContent,
                eventLanguageGroup.Key,
                supportedLanguages.Where(language => language != eventLanguageGroup.Key),
                cancellationToken);

            return translationTask.Select(translatedEvents =>
            {
                var nativeLanguageEvent = translationContent.FirstOrDefault(
                    m => m.UniqueKey == translatedEvents.UniqueKey);
                var createEventDto = createEventRequests.FirstOrDefault(
                    m => m.EventDetails.Name == nativeLanguageEvent!.TranslationPayload.Name);

                return new Event
                {
                    Pl = createEventDto!.Language == Language.Pl
                        ? createEventDto.EventDetails
                        : translatedEvents.Translations[Language.Pl],
                    En = createEventDto.Language == Language.En
                        ? createEventDto.EventDetails
                        : translatedEvents.Translations[Language.En],
                    Url = createEventDto.Url,
                    Date = createEventDto.Date
                };
            });
        });
        var translationResults = await Task.WhenAll(translationTasks);
        return translationResults.SelectMany(x => x);
    }
    
}