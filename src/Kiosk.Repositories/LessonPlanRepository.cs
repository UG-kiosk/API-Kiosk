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
    
    
    public async Task<IEnumerable<string>?> GetMajorItems(string major, int year, string type, IEnumerable<string>? includeGroups, CancellationToken cancellationToken)
    {
        var groupsToExclude = new HashSet<string> { "fakultet","seminarium", "all" };
        var filter = Builders<LessonPlan>.Filter.Eq(lesson => lesson.Name, major) &
                     Builders<LessonPlan>.Filter.Eq(lesson => lesson.Year, year);

        if (type!="seminarium" && type!="fakultet")
            filter &= Builders<LessonPlan>.Filter.Eq(lesson => lesson.Pl.Type, type);

        if (includeGroups != null && includeGroups.Any())
            filter &= Builders<LessonPlan>.Filter.AnyIn(lesson => lesson.Groups, includeGroups);

        if (type != "fakultet")
        {
            var projection = type == "seminarium" ? Builders<LessonPlan>.Projection.Expression(lesson => lesson.Teachers) : Builders<LessonPlan>.Projection.Expression(lesson => lesson.Groups);

            var results = await _lessons.Find(filter)
                .Project(projection)
                .ToListAsync(cancellationToken);
            var flattenedItems = results.SelectMany(x => x).ToList();
            var distinctItems = flattenedItems.Where(item => !groupsToExclude.Contains(item)).Distinct().OrderBy(item => item).ToList();
            return distinctItems;
        }
        else
        {
            var projection = Builders<LessonPlan>.Projection.Expression(lesson => lesson.Pl.Subject);

            var results = await _lessons.Find(filter)
                .Project(projection)
                .ToListAsync(cancellationToken);
            var distinctItems = results
            .Where(subject => !string.IsNullOrEmpty(subject))
            .Distinct()
            .OrderBy(item => item)
            .ToList();
            return distinctItems;
        }
    }
    
    public async Task<IEnumerable<int>?> GetMajorYears(string major, CancellationToken cancellationToken)
        => (await _lessons.DistinctAsync(lessons => lessons.Year, lessons=>lessons.Name==major, cancellationToken: cancellationToken)).ToEnumerable();

}