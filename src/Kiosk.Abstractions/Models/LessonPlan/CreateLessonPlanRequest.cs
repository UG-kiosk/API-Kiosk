using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.LessonPlan;

public class CreateLessonPlanRequest
{
    public required string Name { get; set; }
    
    public required int Year { get; set; }
    
    public required String Day { get; set; }
    
    public required int Start { get; set; }
    
    public required int Duration { get; set; }
    
    public List<string>? Groups { get; set; }
    
    public required List<string> Teachers { get; set; }
    
    public string? Class { get; set; }
    
    public required LessonPlanDetails Details { get; set; }
    
    public required Language SourceLanguage { get; set; }
    
}