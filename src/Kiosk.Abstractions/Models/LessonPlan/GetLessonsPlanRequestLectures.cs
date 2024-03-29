namespace Kiosk.Abstractions.Models.LessonPlan;

public class GetLessonsPlanRequestLectures
{
    public required string Name { get; set; }
    
    public required int Year { get; set; }
}