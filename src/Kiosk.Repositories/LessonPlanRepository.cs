using System.Linq.Expressions;
using Kiosk.Abstractions.Models.LessonPlan;
using Kiosk.Abstractions.Models.Pagination;
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
        var groups = new[] { "all", getLessonPlanRequest.PracticalClasses, getLessonPlanRequest.Labs,  getLessonPlanRequest.ForeignLanguage };
        var filter = Builders<LessonPlan>.Filter.Where(lessons =>
            lessons.Name == getLessonPlanRequest.Name &&
            lessons.Year == getLessonPlanRequest.Year &&
            (lessons.Groups != null 
             && (lessons.Groups.Any(group => groups.Contains(group)) 
             || getLessonPlanRequest.Seminars!=null && lessons.Groups.Contains("seminarium") && lessons.Teachers.Contains(getLessonPlanRequest.Seminars)
             || getLessonPlanRequest.Faculties != null && getLessonPlanRequest.Faculties!.Count > 0 && getLessonPlanRequest.Faculties.Contains(lessons.Pl.Subject)
             )
             ));
        
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

    public async Task CreateLessons(IEnumerable<LessonPlan> mappedLessons, CancellationToken cancellationToken)
    {
        await _lessons.InsertManyAsync(mappedLessons, cancellationToken: cancellationToken);
    }

    public async Task<(IEnumerable<LessonPlan>, Pagination Pagination)> GetLessons(string? day, string? search,
        Pagination pagination, CancellationToken cancellationToken)
    {
        Expression<Func<LessonPlan, bool>> filter = lesson => 
            (search == null || lesson.Name.Contains(search) || lesson.Teachers.Contains(search))
            && (day == null || lesson.Day.Contains(day));


        var lessons = await _lessons.Find(filter).Skip((pagination.Page - 1) * pagination.ItemsPerPage)
            .Limit(pagination.ItemsPerPage)
            .ToListAsync(cancellationToken);
        
        var totalStaffRecords = await _lessons.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        pagination.TotalPages = Pagination.CalculateTotalPages((int)totalStaffRecords, pagination.ItemsPerPage);
        pagination.HasNextPage = Pagination.CalculateHasNextPage(pagination.Page, pagination.TotalPages);
        return (lessons, pagination);
    }

    public async Task<LessonPlan?> DeleteLesson(string lessonsId, CancellationToken cancellationToken)
        => await _lessons.FindOneAndDeleteAsync(l => l._id == lessonsId, cancellationToken: cancellationToken);
}