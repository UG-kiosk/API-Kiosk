using System.Text.Json.Serialization;
using Kiosk.Abstractions.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models.Major;


public class MajorDetails
{
    public required string Name { get; set; }
    
    public required string? Content { get; set; }
}

public class MajorDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    
    public required MajorDetails Pl { get; set; }
    
    public required MajorDetails En { get; set; }
    
    public required string? Url { get; set; }
   
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Degree Degree { get; set; }
}