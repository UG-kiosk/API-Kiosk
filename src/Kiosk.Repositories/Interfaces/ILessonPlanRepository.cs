using Kiosk.Abstractions.Models.LessonPlan;
using MongoDB.Driver;

namespace Kiosk.Repositories.Interfaces;

public interface ILessonPlanRepository
{
    Task<IEnumerable<LessonPlan>?> GetAllLecturesForMajorYear(GetLessonsPlanRequestLectures getLessonsPlanRequestLectures, CancellationToken cancellationToken);
    Task<IEnumerable<LessonPlan>?> GetAllLessonsForMajorYearGroup(GetLessonPlanRequest getLessonPlanRequest, CancellationToken cancellationToken);

}