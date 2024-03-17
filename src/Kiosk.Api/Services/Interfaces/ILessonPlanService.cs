using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.LessonPlan;

namespace KioskAPI.Services.Interfaces;

public interface ILessonPlanService
{
    Task<IEnumerable<LessonPlanResponse>?> GetAllLessonsForMajorYear(BaseLessonPlanRequest baseLessonPlanRequest, Language language, CancellationToken cancellationToken);
    
    Task<IEnumerable<LessonPlanResponse>?> GetAllLecturesForMajorYear(BaseLessonPlanRequest baseLessonPlanRequest, Language language, CancellationToken cancellationToken);
    
    Task<IEnumerable<LessonPlanResponse>?> GetAllLessonsForMajorYearGroup(LessonPlanGroupRequest lessonPlanGroupRequest, Language language, CancellationToken cancellationToken);

}