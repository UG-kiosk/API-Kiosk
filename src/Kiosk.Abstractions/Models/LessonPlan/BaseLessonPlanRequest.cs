namespace Kiosk.Abstractions.Models.LessonPlan;

public class BaseLessonPlanRequest
{
    public required string Major { get; set; }
    
    public required int Year { get; set; }
}