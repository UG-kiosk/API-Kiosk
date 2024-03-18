using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Events;

namespace KioskAPI.Services.Interfaces;

public interface IEventsService
{
    Task<EventResponse?> GetTranslatedEvent(string eventId, Language language, CancellationToken cancellationToken);
    Task<IEnumerable<EventResponse>> GetTranslatedEvents(Source? source, Language language, CancellationToken cancellationToken);
}