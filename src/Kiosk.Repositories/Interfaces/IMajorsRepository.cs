using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Major;

namespace Kiosk.Repositories.Interfaces;

public interface IMajorsRepository
{
    Task<Major?> GetMajor(string id, CancellationToken cancellationToken);
    Task<IEnumerable<Major>> GetMajors(FindMajorsRequest findMajorsRequest, CancellationToken cancellationToken);
    Task<Major?> DeleteMajor(string id, CancellationToken cancellationToken);
}