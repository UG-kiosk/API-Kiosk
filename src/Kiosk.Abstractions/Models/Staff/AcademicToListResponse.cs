using System.Text.Json.Serialization;
using Kiosk.Abstractions.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models.Staff;

public class AcademicToListResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Link { get; set; }
    
    public required List<string> Units { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Language Language { get; set; }
}