using Kiosk.Abstractions.Models.Major;

namespace Kiosk.Repositories.Interfaces;

public interface IMajorsRepository
{
    Task<MajorDocument?> GetMajor(string id, CancellationToken cancellationToken);
    Task<IEnumerable<MajorDocument>> GetMajors(FindMajorsRequest findMajorsRequest, CancellationToken cancellationToken);
    Task<MajorDocument?> DeleteMajor(string id, CancellationToken cancellationToken);
    Task CreateMajors(IEnumerable<MajorDocument> majorDocuments, CancellationToken cancellationToken);
}