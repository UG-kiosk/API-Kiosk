

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models.Events;

public class EventDetails
{
    public required string Name { get; set; }
    public required Object Content { get; set; }
}

public class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    public required EventDetails Pl { get; set; }
    public required EventDetails En { get; set; }
    
    public required string? Url { get; set; }
}