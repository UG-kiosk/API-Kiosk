using Kiosk.Abstractions.Models.LessonPlan;
using MongoDB.Driver;

namespace Kiosk.Repositories.Interfaces;

public interface ILessonPlanRepository
{
    Task<IEnumerable<LessonPlan>?> GetAllLecturesForMajorYear(GetLessonsPlanRequestLectures getLessonsPlanRequestLectures, CancellationToken cancellationToken);
    Task<IEnumerable<LessonPlan>?> GetAllLessonsForMajorYearGroup(GetLessonPlanRequest getLessonPlanRequest, CancellationToken cancellationToken);
    Task<IEnumerable<string>?> GetMajors(CancellationToken cancellationToken);
    Task<IEnumerable<string>?> GetMajorItems(string major, int year, string type, IEnumerable<string>? includeGroups,
        CancellationToken cancellationToken);

    Task<IEnumerable<int>?> GetMajorYears(string major, CancellationToken cancellationToken);

    Task CreateLessons(IEnumerable<LessonPlan> mappedLessons, CancellationToken cancellationToken);
}