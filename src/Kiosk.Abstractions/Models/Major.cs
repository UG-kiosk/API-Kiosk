using Kiosk.Abstractions.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models;


public class MajorDetails
{
    [BsonElement("name")]
    public required string Name { get; set; }
    
    [BsonElement("content")]
    public required string? Content { get; set; }
}

public class Major
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    
    [BsonElement("pl")]
    public required MajorDetails PolishDetails { get; set; }
    
    [BsonElement("en")]
    public required MajorDetails EnglishDetails { get; set; }
    
    [BsonElement("url")]
    public required string? Url { get; set; }
   
    [BsonElement("degree")]
    public required Degree Degree { get; set; }
}