namespace Kiosk.Abstractions.Models.LessonPlan;

public class GetLessonPlanRequest : GetLessonsPlanRequestLectures
{
    public string? Group { get; set; } = null;
}