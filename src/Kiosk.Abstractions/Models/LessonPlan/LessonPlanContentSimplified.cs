namespace Kiosk.Abstractions.Models.LessonPlan;

public class LessonPlanContentSimplified
{
    public required string Subject { get; set; }
    
    public string? Type { get; set; }
    
    public string? Info { get; set; }
}