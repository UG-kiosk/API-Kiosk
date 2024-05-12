namespace Kiosk.Abstractions.Models.LessonPlan;

public class GetLessonPlanRequest : GetLessonsPlanRequestLectures
{
    public string? PracticalClasses { get; set; } = null;
    public string? Labs { get; set; } = null;
    public List<string>? Faculties { get; set; } = null;
    public string? Seminars { get; set; } = null;
    public string? ForeignLanguage { get; set; } = null;

}