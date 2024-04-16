using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Events;

namespace Kiosk.Repositories.Interfaces;

public interface IEventsRepository
{
    Task<Event?> GetEvent(string id, CancellationToken cancellationToken);
    
    Task<IEnumerable<Event>?> GetManyEvents(CancellationToken cancellationToken);
}