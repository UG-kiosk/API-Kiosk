using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.LessonPlan;

namespace KioskAPI.Services.Interfaces;

public interface ILessonPlanService
{
    Task<IEnumerable<GetLessonPlanResponse>?> GetAllLecturesForMajorYear(GetLessonsPlanRequestLectures getLessonsPlanRequestLectures, Language language, CancellationToken cancellationToken);
    
    Task<IEnumerable<GetLessonPlanResponse>?> GetAllLessonsForMajorYearGroup(GetLessonPlanRequest getLessonPlanRequest, Language language, CancellationToken cancellationToken);

}