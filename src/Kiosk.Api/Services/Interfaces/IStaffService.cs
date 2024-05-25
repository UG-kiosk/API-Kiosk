using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.Staff;

namespace KioskAPI.Services.Interfaces;

public interface IStaffService
{  
    Task<(IEnumerable<AcademicToListResponse>, Pagination Pagination)> GetStaff(Language language, int? page, int? itemsPerPage, string? name, CancellationToken cancellationToken);
    Task<AcademicResponse?> GetStaffMember(string academicId, Language language, CancellationToken cancellationToken);
    Task CreateStaff(IEnumerable<AcademicRequest> staff, CancellationToken cancellationToken);
    Task<AcademicResponse?> UpdateStaffMember(string academicId, AcademicRequest academic, CancellationToken cancellationToken);
}