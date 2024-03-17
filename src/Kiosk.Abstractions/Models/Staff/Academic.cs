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
    
    [BsonIgnore]
    public AcademicContent this[string language]
    {
        get
        {
            switch (language)
            {
                case "Pl":
                    return Pl;
                case "En":
                    return En;
                default:
                    throw new ArgumentException($"Invalid language: {language}");
            }
        }
    }
}