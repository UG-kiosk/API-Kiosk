using Kiosk.Abstractions.Models;

namespace Kiosk.Repositories.Interfaces;

public interface IEctsSubjectRepository
{
    Task CreateEctsSubject(EctsSubject ectsSubject, CancellationToken cancellationToken);

    Task<EctsSubject?> DeleteEctsSubject(string id, CancellationToken cancellationToken);

    Task<EctsSubject?> UpdateEctsSubject(EctsSubject ectsSubject, CancellationToken cancellationToken);

    Task<IEnumerable<EctsSubject>> GetEctsSubjects(CancellationToken cancellationToken);

    Task<IEnumerable<EctsSubject>?> GetEctsSubjectsByName(string subject, CancellationToken cancellationToken);
}