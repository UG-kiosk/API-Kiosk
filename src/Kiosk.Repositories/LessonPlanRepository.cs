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
    

    public async Task<IEnumerable<LessonPlan>?> GetAllLecturesForMajorYear(GetLessonsPlanRequestLectures getLessonsPlanRequestLectures, CancellationToken cancellationToken)
    {
        var filter = Builders<LessonPlan>.Filter.Where(lessons =>
            lessons.Name == getLessonsPlanRequestLectures.Name &&
            lessons.Year == getLessonsPlanRequestLectures.Year &&
            lessons.Pl.Type=="wykład");
        
        return await _lessons.Find(filter)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LessonPlan>?> GetAllLessonsForMajorYearGroup(GetLessonPlanRequest getLessonPlanRequest, CancellationToken cancellationToken)
    {
        var lessonsType = new[] { "all", getLessonPlanRequest.Group, "fakultet", "seminarium" };
        var filter = Builders<LessonPlan>.Filter.Where(lessons =>
            lessons.Name == getLessonPlanRequest.Name &&
            lessons.Year == getLessonPlanRequest.Year &&
            (getLessonPlanRequest.Group == null 
             || lessons.Groups != null 
             && lessons.Groups.Any(group => lessonsType.Contains(group))));
        
        return await _lessons.Find(filter)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<string>?> GetMajors(CancellationToken cancellationToken)
        => (await _lessons.DistinctAsync(lessons => lessons.Name, _ => true, cancellationToken: cancellationToken)).ToEnumerable();

}