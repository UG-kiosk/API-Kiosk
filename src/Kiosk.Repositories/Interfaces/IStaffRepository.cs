using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Staff;
using MongoDB.Driver;

namespace Kiosk.Repositories.Interfaces;

public interface IStaffRepository
{
    Task<(IEnumerable<Academic> Staff, Pagination Pagination)> GetStaff(ProjectionDefinition<Academic> projection,
        Pagination pagination, FilterDefinition<Academic> filter, CancellationToken cancellationToken);
    
    Task<Academic?> GetAcademic(string academicId, ProjectionDefinition<Academic> projection,
        CancellationToken cancellationToken);
}