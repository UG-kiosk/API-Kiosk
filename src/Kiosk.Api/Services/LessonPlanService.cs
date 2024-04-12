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
    
    private GetLessonPlanResponse MapTranslatedLessons(LessonPlan lessonPlan, Language language)
    {
        var mappedLessons = _mapper.Map<GetLessonPlanResponse>(lessonPlan);
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

    public async Task<IEnumerable<GetLessonPlanResponse>?> GetAllLecturesForMajorYear(GetLessonsPlanRequestLectures getLessonsPlanRequestLectures, Language language, CancellationToken cancellationToken)
    {
        var lessons = await _lessonPlanRepository.GetAllLecturesForMajorYear(getLessonsPlanRequestLectures, cancellationToken);
        return lessons?.Select(lesson => MapTranslatedLessons(lesson, language));
    }

    public async Task<IEnumerable<GetLessonPlanResponse>?> GetAllLessonsForMajorYearGroup(GetLessonPlanRequest getLessonPlanRequest, Language language, CancellationToken cancellationToken)
    {
        
        var lessons = await _lessonPlanRepository.GetAllLessonsForMajorYearGroup(getLessonPlanRequest, cancellationToken);
        return lessons?.Select(lesson => MapTranslatedLessons(lesson, language));
    }

    public Task<IEnumerable<string>?> GetMajors(CancellationToken cancellationToken)
    {
        var result = _lessonPlanRepository.GetMajors(cancellationToken);
        return result;
    }

    public async Task<GetMajorGroupsResponse> GetMajorGroups(string major, int year, CancellationToken cancellationToken)
    {
        var practicalClassesGroups = _lessonPlanRepository.GetMajorItems(major, year, "ćwiczenia", null, cancellationToken);
        var labGroups = _lessonPlanRepository.GetMajorItems(major, year, "laboratorium", null, cancellationToken);
        var seminarGroups = _lessonPlanRepository.GetMajorItems(major, year, "seminarium", new List<string> { "seminarium" }, cancellationToken);
        var facultyGroups = _lessonPlanRepository.GetMajorItems(major, year, "fakultet", new List<string> { "fakultet" }, cancellationToken);
        var foreignLanguage =  _lessonPlanRepository.GetMajorItems(major, year, "lektorat", null, cancellationToken);
        
        await Task.WhenAll(practicalClassesGroups, labGroups, seminarGroups, facultyGroups, foreignLanguage);

        return new GetMajorGroupsResponse
        {
            PracticalClasses = (await practicalClassesGroups)?.ToList(),
            Labs = (await labGroups)?.ToList(),
            Seminars = (await seminarGroups)?.ToList(),
            Faculties = (await facultyGroups)?.ToList(),
            ForeignLanguage = (await foreignLanguage)?.ToList()
        };
    }
    
    public Task<IEnumerable<int>?> GetMajorYears(string major, CancellationToken cancellationToken)
    {
        var result = _lessonPlanRepository.GetMajorYears(major, cancellationToken);
        return result;
    }
}