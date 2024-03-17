namespace Kiosk.Abstractions.Models.LessonPlan;

public class BaseLessonPlanRequest
{
    public required string Name { get; set; }
    
    public required int Year { get; set; }
}