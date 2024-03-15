using System.Text.Json.Serialization;
using Kiosk.Abstractions.Enums.News;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models.News;

public class NewsDetails
{
    public required string Title { get; set; }
    
    public required string ShortBody { get; set; }
    
    public required string Body { get; set; }
}

public class News
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    
    public required string LeadingPhoto { get; set; }
    
    public required List<string> Photos { get; set; }
    
    public required string Link { get; set; }
    
    public required DateOnly Datetime { get; set; }
    
    public required NewsDetails Pl { get; set; }
    
    public required NewsDetails En { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Source Source { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Category Category { get; set; }
}