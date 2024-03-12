using Kiosk.Abstractions.Dtos;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;

namespace Kiosk.Repositories.Interfaces;

public interface IMajorsRepository
{
    Task<Major?> GetMajor(string id, CancellationToken cancellationToken);
    Task<IEnumerable<Major>> GetMajors(FindMajorsQueryDto findMajorsQueryDto, CancellationToken cancellationToken);
    Task<Major?> DeleteMajor(string id, CancellationToken cancellationToken);
}