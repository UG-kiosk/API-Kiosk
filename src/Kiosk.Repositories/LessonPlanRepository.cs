using Kiosk.Abstractions.Models.LessonPlan;
using Kiosk.Repositories.Interfaces;
using MongoDB.Driver;

namespace Kiosk.Repositories;

public class LessonPlanRepository : ILessonPlanRepository
{
    private readonly string _collectionName = "lessons";
    private readonly IMongoCollection<LessonPlan> _lessons;

    public LessonPlanRepository(IMongoDatabase mongoDatabase)
    {
        _lessons = mongoDatabase.GetCollection<LessonPlan>(_collectionName);
    }


    public async Task<IEnumerable<LessonPlan>?> GetLessons(FilterDefinition<LessonPlan> filter, CancellationToken cancellationToken)
        => await _lessons.Find(filter)
            .ToListAsync(cancellationToken);
}