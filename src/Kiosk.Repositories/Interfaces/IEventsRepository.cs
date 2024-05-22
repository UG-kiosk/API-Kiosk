using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Events;
using Kiosk.Abstractions.Models.Pagination;

namespace Kiosk.Repositories.Interfaces;

public interface IEventsRepository
{
    Task<Event?> GetEvent(string id, CancellationToken cancellationToken);
    
    Task<(IEnumerable<Event>?, Pagination Pagination)> GetManyEvents(Pagination pagination ,CancellationToken cancellationToken);
    
    Task CreateEvent(IEnumerable<Event> events, CancellationToken cancellationToken);
}