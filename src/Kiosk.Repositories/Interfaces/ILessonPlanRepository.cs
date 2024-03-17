using Kiosk.Abstractions.Models.LessonPlan;
using MongoDB.Driver;

namespace Kiosk.Repositories.Interfaces;

public interface ILessonPlanRepository
{
    Task<IEnumerable<LessonPlan>?> GetLessons(FilterDefinition<LessonPlan> filter, CancellationToken cancellationToken);
}