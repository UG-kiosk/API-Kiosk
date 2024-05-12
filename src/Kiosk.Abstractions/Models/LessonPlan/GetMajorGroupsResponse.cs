namespace Kiosk.Abstractions.Models.LessonPlan;

public class GetMajorGroupsResponse
{
    public List<string>? PracticalClasses { get; set; }
    public List<string>? Labs { get; set; }
    public List<string>? Faculties { get; set; }
    public List<string>? Seminars { get; set; }
    public List<string>? ForeignLanguage { get; set; }

}