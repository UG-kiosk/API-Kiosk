using System.Text.Json.Serialization;
using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.LessonPlan;

public class LessonPlanResponse
{
    public string? _id { get; set; }
    
    public required string Name { get; set; }
    
    public required int Year { get; set; }
    
    public required DayOfWeek Day { get; set; }
    
    public required int Start { get; set; }
    
    public required int Duration { get; set; }
    
    public List<string>? Groups { get; set; }
    
    public required List<string> Teachers { get; set; }
    
    public required object Class { get; set; }
    
    public required string Subject { get; set; }
    
    public required string Type { get; set; }
    
    public List<string>? Info { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Language Language { get; set; }

}