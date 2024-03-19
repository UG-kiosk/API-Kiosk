using Kiosk.Abstractions.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models.Staff;

[BsonIgnoreExtraElements]
public class Academic
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Link { get; set; }
    
    public required string Email { get; set; }

    public required AcademicContent Pl { get; set; }
    
    public AcademicContent En { get; set; }
    
    public AcademicContent this[Language language]
    {
        get
        {
            return language switch
            {
                Language.Pl => Pl,
                Language.En => En,
                _ => throw new ArgumentException($"Invalid language: {language}")
            };
        }
    }
}