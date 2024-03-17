using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Staff;

namespace KioskAPI.Services.Interfaces;

public interface IStaffService
{  
    Task<(IEnumerable<AcademicResponse>, Pagination Pagination)> GetStaff(Language language,
        CancellationToken cancellationToken, int page, int itemsPerPage, string name);
    
    Task<AcademicResponse?> GetStaffMember(string academicId, Language language, CancellationToken cancellationToken);
}