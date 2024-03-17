using Kiosk.Abstractions.Models.Staff;
using MongoDB.Driver;

namespace Kiosk.Repositories.Interfaces;

public interface IStaffRepository
{
    Task<IEnumerable<Academic>> GetStaff(ProjectionDefinition<Academic> projection, CancellationToken cancellationToken);
    
    Task<Academic?> GetAcademic(string academicId, ProjectionDefinition<Academic> projection, CancellationToken cancellationToken);
}