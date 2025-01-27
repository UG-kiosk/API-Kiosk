using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Major;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models;

public class EctsSubjectDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    
    public required EctsSubjectDetails Pl { get; set; }
    
    public required EctsSubjectDetails En { get; set; }
    
    public required int LectureHours { get; set; }

    public required int RecitationHours { get; set; }

    public required int LabsHours { get; set; }

    public string? Pass { get; set; }

    public required int Ects { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Degree Degree { get; set; }

    [Range(1, 6)] public required int Term { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Range(1, 3)] public required Year Year { get; set; }

    public required List<int> RecruitmentYear { get; set; }
    
    public EctsSubjectDetails this[Language language]
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

public class EctsSubjectDetails
{
    public required string Subject { get; set; }
    
    public required string Major { get; set; }

    public string? Speciality { get; set; }
}
