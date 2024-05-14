using System.Linq.Expressions;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Events;
using MongoDB.Driver;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Repositories.Interfaces;
using MongoDB.Driver;


namespace Kiosk.Repositories.Interfaces;

public class EventsRepository : IEventsRepository
{
    private readonly string _collectionName = "events";
    private readonly IMongoCollection<Event> _eventsCollection;
    
    public EventsRepository(IMongoDatabase mongoDatabase)
    {
        _eventsCollection = mongoDatabase.GetCollection<Event>(_collectionName);
    }
    
    public async Task<Event?> GetEvent(string id, CancellationToken cancellationToken)
        => await _eventsCollection.Find(events => events._id == id)
            .SortByDescending(events => events.Date)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Event>?> GetManyEvents(CancellationToken cancellationToken)
    => await _eventsCollection.Find(Builders<Event>.Filter.Empty).ToListAsync(cancellationToken);
}