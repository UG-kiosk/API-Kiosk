using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Events;
using Kiosk.Abstractions.Models.Pagination;

namespace KioskAPI.Services.Interfaces;

public interface IEventsService
{
    Task<EventResponse?> GetTranslatedEvent(string eventId, Language language, CancellationToken cancellationToken);
    
    
    Task<(IEnumerable<EventResponse>?, Pagination Pagination)> GetTranslatedEvents(Language language, int? page, int? itemsPerPage, CancellationToken cancellationToken);
    
    Task CreateEvent(IEnumerable<EventRequest> createEventRequests, CancellationToken cancellationToken);
    Task<EventResponse?> UpdateEvent(string eventId, EventRequest eventRequest, CancellationToken cancellationToken);
}