using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.LessonPlan;
using Kiosk.Abstractions.Models.Pagination;

namespace KioskAPI.Services.Interfaces;

public interface ILessonPlanService
{
    Task<IEnumerable<GetLessonPlanResponse>?> GetAllLecturesForMajorYear(GetLessonsPlanRequestLectures getLessonsPlanRequestLectures, Language language, CancellationToken cancellationToken);
    
    Task<IEnumerable<GetLessonPlanResponse>?> GetAllLessonsForMajorYearGroup(GetLessonPlanRequest getLessonPlanRequest, Language language, CancellationToken cancellationToken);

    Task<IEnumerable<string>?> GetMajors(CancellationToken cancellationToken);

    Task<GetMajorGroupsResponse> GetMajorGroups(string major, int year, CancellationToken cancellationToken);
    Task<IEnumerable<int>?> GetMajorYears(string major, CancellationToken cancellationToken);

    Task CreateLessons(IEnumerable<CreateLessonPlanRequest> createLessonPlanRequests, CancellationToken cancellationToken);
    Task<(IEnumerable<GetLessonPlanResponse?>, Pagination Pagination)> GetAllLessons(string? day, string? search,
        Language language, PaginationRequest paginationRequest,
        CancellationToken cancellationToken);
}