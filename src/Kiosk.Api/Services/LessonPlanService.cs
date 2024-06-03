using System.Collections.Immutable;
using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.LessonPlan;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.Translation;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class LessonPlanService : ILessonPlanService
{
    private readonly ILessonPlanRepository _lessonPlanRepository;
    private readonly IMapper _mapper;
    private readonly ITranslatorService _translatorService;

    public LessonPlanService(ILessonPlanRepository lessonPlanRepository, IMapper mapper, ITranslatorService translatorService)
    {
        _lessonPlanRepository = lessonPlanRepository;
        _mapper = mapper;
        _translatorService = translatorService;
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

    public async Task CreateLessons(IEnumerable<CreateLessonPlanRequest> createLessonPlanRequests, CancellationToken cancellationToken)
    {
        var mappedLessons = await TranslateLessons(createLessonPlanRequests, cancellationToken);
        await _lessonPlanRepository.CreateLessons(mappedLessons, cancellationToken);
    }

    public async Task<(IEnumerable<GetLessonPlanResponse>?,Pagination Pagination)> GetAllLessons(Language language, PaginationRequest paginationRequest, CancellationToken cancellationToken)
    {
        var pagination = new Pagination
        {
            Page = paginationRequest.Page,
            ItemsPerPage = paginationRequest.ItemsPerPage
        };
        
        var (lessonList, updatedPagination) = await _lessonPlanRepository.GetLessons(pagination, cancellationToken);

        return (lessonList?.Select(lesson => MapTranslatedLessons(lesson, language)), updatedPagination);
    }

    private async Task<IEnumerable<LessonPlan>> TranslateLessons(IEnumerable<CreateLessonPlanRequest> createLessonPlanRequests, CancellationToken cancellationToken)
    {
        ImmutableList<Language> supportedLanguages = new List<Language> { Language.En, Language.Pl }.ToImmutableList();

        var simplifiedLessons = createLessonPlanRequests
            .Select(lessonGroup => _mapper.Map<CreateLessonPlanSimplified>(lessonGroup)).ToList();
        
        var translationContent = simplifiedLessons.Select(lessonsGroup => new TranslationRequest<LessonPlanContentSimplified>
        {
            UniqueKey = Guid.NewGuid().ToString(),
            TranslationPayload = lessonsGroup.Details
        }).ToList();
        
        var translationTask = await _translatorService.Translate(
            translationContent,
            Language.Pl,
            supportedLanguages.Where(language => language != Language.Pl),
            cancellationToken);
        
        var translatedLessons = translationTask.Select(translatedContent =>
        {
            var originalContent = simplifiedLessons.First(m =>
                m.Details == translationContent.First(n => n.UniqueKey == translatedContent.UniqueKey).TranslationPayload);
            return new LessonPlan
            {
                Name = originalContent.Name,
                Year = originalContent.Year,
                Day = originalContent.Day,
                Start = originalContent.Start,
                Duration = originalContent.Duration,
                Groups = originalContent.Groups,
                Teachers = originalContent.Teachers,
                Class = originalContent.Class,
                Pl = _mapper.Map<LessonPlanDetails>(originalContent.Details),
                En = _mapper.Map<LessonPlanDetails>(translatedContent.Translations[Language.En]),
            };
        });
        return translatedLessons;
            
    }
}