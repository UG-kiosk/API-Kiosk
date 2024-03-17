using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.LessonPlan;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;
using MongoDB.Driver;

namespace KioskAPI.Services;

public class LessonPlanService : ILessonPlanService
{
    private readonly ILessonPlanRepository _lessonPlanRepository;
    private readonly IMapper _mapper;

    public LessonPlanService(ILessonPlanRepository lessonPlanRepository, IMapper mapper)
    {
        _lessonPlanRepository = lessonPlanRepository;
        _mapper = mapper;
    }
    
    private LessonPlanResponse MapTranslatedLessons(LessonPlan lessonPlan, Language language)
    {
        var mappedLessons = _mapper.Map<LessonPlanResponse>(lessonPlan);
        mappedLessons.Language = language;

        switch (language)
        {
            case Language.Pl:
                mappedLessons.Subject = lessonPlan.Pl.Subject;
                mappedLessons.Type = lessonPlan.Pl.Type;
                mappedLessons.Info = lessonPlan.Pl.Info;
                break;
            case Language.En:
                mappedLessons.Subject = lessonPlan.En.Subject;
                mappedLessons.Type = lessonPlan.En.Type;
                mappedLessons.Info = lessonPlan.En.Info;
                break;
        }

        return mappedLessons;
    }
    
    public async Task<IEnumerable<LessonPlanResponse>?> GetAllLessonsForMajorYear(BaseLessonPlanRequest baseLessonPlanRequest, Language language, CancellationToken cancellationToken)
    {
        var filter = Builders<LessonPlan>.Filter.Where(lessons => 
                lessons.Name == baseLessonPlanRequest.Name &&
                lessons.Year == baseLessonPlanRequest.Year);
        var lessons = await _lessonPlanRepository.GetLessons(filter, cancellationToken);
        return lessons?.Select(lesson => MapTranslatedLessons(lesson, language));
    }

    public async Task<IEnumerable<LessonPlanResponse>?> GetAllLecturesForMajorYear(BaseLessonPlanRequest baseLessonPlanRequest, Language language, CancellationToken cancellationToken)
    {
        var filter = Builders<LessonPlan>.Filter.Where(lessons =>
                lessons.Name == baseLessonPlanRequest.Name &&
                lessons.Year == baseLessonPlanRequest.Year &&
                lessons.Pl.Type=="wykład");
        var lessons = await _lessonPlanRepository.GetLessons(filter, cancellationToken);
        return lessons?.Select(lesson => MapTranslatedLessons(lesson, language));
    }

    public async Task<IEnumerable<LessonPlanResponse>?> GetAllLessonsForMajorYearGroup(LessonPlanGroupRequest lessonPlanGroupRequest, Language language, CancellationToken cancellationToken)
    {
        var lessonsType = new[] { "all", lessonPlanGroupRequest.Group, "fakultet", "seminarium" };
        var filter = Builders<LessonPlan>.Filter.Where(lessons =>
            lessons.Name == lessonPlanGroupRequest.Name &&
            lessons.Year == lessonPlanGroupRequest.Year &&
            lessons.Groups != null &&
            lessons.Groups.Any(group => lessonsType.Contains(group)));
        var lessons = await _lessonPlanRepository.GetLessons(filter, cancellationToken);
        return lessons?.Select(lesson => MapTranslatedLessons(lesson, language));
    }
}