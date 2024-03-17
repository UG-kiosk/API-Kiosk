using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Staff;

namespace KioskAPI.Services.Interfaces;

public interface IStaffService
{  
    Task<IEnumerable<AcademicResponse>> GetStaff(Language language, CancellationToken cancellationToken);
    
    Task<AcademicResponse?> GetStaffMember(string academicId, Language language, CancellationToken cancellationToken);
}