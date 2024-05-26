using System.Linq.Expressions;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Events;
using MongoDB.Driver;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Pagination;
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

    
    
    public async Task<(IEnumerable<Event>?, Pagination Pagination)> GetManyEvents(Pagination pagination, CancellationToken cancellationToken) 
    {
        var filter = Builders<Event>.Filter.Empty;
        var events = await _eventsCollection.Find(filter)
            .SortByDescending(events => events.Date)
            .Skip((pagination.Page - 1) * pagination.ItemsPerPage)
            .Limit(pagination.ItemsPerPage)
            .ToListAsync(cancellationToken);
        return (events, pagination);
    }
    public async Task CreateEvent(IEnumerable<Event> events, CancellationToken cancellationToken)
    {
        await _eventsCollection.InsertManyAsync(events, cancellationToken: cancellationToken);
    }
    
    public async Task<Event?> UpdateEvent(string eventId, Event eventRequest, CancellationToken cancellationToken)
    {
        var update = Builders<Event>.Update
            .Set(events => events.Pl, eventRequest.Pl)
            .Set(events => events.En, eventRequest.En)
            .Set(events => events.Date, eventRequest.Date);
        await _eventsCollection.UpdateOneAsync(events => events._id == eventId, update, cancellationToken: cancellationToken);
        var updatedEvent = await _eventsCollection.Find(events => events._id == eventId)
            .FirstOrDefaultAsync(cancellationToken);
        
        return updatedEvent;
    }
    
    public async Task<Event?> DeleteEvent(string eventId, CancellationToken cancellationToken)
    {
        return await _eventsCollection.FindOneAndDeleteAsync(events => events._id == eventId, cancellationToken: cancellationToken);
    }
    
    
}