using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models.LessonPlan;

public class LessonPlanDetails
{
    public required string Subject { get; set; }
    
    public string? Type { get; set; }
    
    public List<string>? Info { get; set; }
}

[BsonIgnoreExtraElements]
public class LessonPlan
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }
    
    public required string Name { get; set; }
    
    public required int Year { get; set; }
    
    public required String Day { get; set; }
    
    public required int Start { get; set; }
    
    public required int Duration { get; set; }
    
    public List<string>? Groups { get; set; }
    
    public required List<string> Teachers { get; set; }
    
    public string? Class { get; set; }
    
    public required LessonPlanDetails Pl { get; set; }
    
    public required LessonPlanDetails En { get; set; }
}