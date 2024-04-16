using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Events;

namespace KioskAPI.Services.Interfaces;

public interface IEventsService
{
    Task<GetEventResponse?> GetTranslatedEvent(string eventId, Language language, CancellationToken cancellationToken);
    
    
    Task<IEnumerable<GetEventResponse>> GetTranslatedEvents(Language language, CancellationToken cancellationToken);
}