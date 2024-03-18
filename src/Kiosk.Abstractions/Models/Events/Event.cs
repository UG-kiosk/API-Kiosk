

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models.Events;

public class EventDetails
{
    public required string name { get; set; }
    public required string content { get; set; }
}

public class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    
    public string? _id { get; set; }
    
    public required string url { get; set; }
    
    public required EventDetails Pl { get; set; }
    
}