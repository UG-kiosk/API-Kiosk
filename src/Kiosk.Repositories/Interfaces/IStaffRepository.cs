using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.Staff;

namespace Kiosk.Repositories.Interfaces;

public interface IStaffRepository
{
    Task<(IEnumerable<Academic> Staff, Pagination Pagination)> GetStaff(Language language, Pagination pagination, string? name, CancellationToken cancellationToken);
    Task<Academic?> GetAcademic(string academicId, Language language, CancellationToken cancellationToken);
    Task CreateStaff(IEnumerable<Academic> staff, CancellationToken cancellationToken);
    Task<Academic?> UpdateStaffMember(string academicId, Academic academic, CancellationToken cancellationToken);
    Task<Academic?> DeleteStaffMember(string academicId, CancellationToken cancellationToken);
}
