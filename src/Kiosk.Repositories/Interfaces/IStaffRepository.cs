using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.Staff;
using MongoDB.Driver;

namespace Kiosk.Repositories.Interfaces;

public interface IStaffRepository
{
    Task<(IEnumerable<Academic> Staff, Pagination Pagination)> GetStaff(Language language, Pagination pagination, string? name, CancellationToken cancellationToken);
    
    Task<Academic?> GetAcademic(string academicId, Language language, CancellationToken cancellationToken);
}