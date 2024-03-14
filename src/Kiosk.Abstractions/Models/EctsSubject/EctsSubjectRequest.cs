using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models;

public class EctsSubjectRequest
{
    public required Degree Degree { get; set; }

    public required string? Major { get; set; }
    
    public required int Year { get; set; }
    
    public string? Speciality { get; set; }
}