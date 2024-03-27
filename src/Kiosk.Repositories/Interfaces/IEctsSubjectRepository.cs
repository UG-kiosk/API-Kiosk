using System.Collections;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;

namespace Kiosk.Repositories.Interfaces;

public interface IEctsSubjectRepository
{
    Task CreateEctsSubject(EctsSubjectDocument ectsSubjectDocument, CancellationToken cancellationToken);

    Task<EctsSubjectDocument?> DeleteEctsSubject(string id, CancellationToken cancellationToken);

    Task<EctsSubjectDocument?> UpdateEctsSubject(EctsSubjectDocument ectsSubjectDocument, CancellationToken cancellationToken);

    Task<IEnumerable<EctsSubjectDocument>> GetEctsSubjects(CancellationToken cancellationToken);

    Task<IEnumerable<EctsSubjectDocument>?> GetEctsSubjectsByName(string subject, CancellationToken cancellationToken);

    Task<IEnumerable<string>?> GetMajors(Degree degree, CancellationToken cancellationToken);

    Task<IEnumerable<EctsSubjectDocument>?> GetSubjectsByMajor(EctsSubjectRequest ectsSubject, CancellationToken cancellationToken);

    Task<IEnumerable<int>?> GetYears(BaseEctsSubjectRequest baseEctsSubjectRequest, CancellationToken cancellationToken);

    Task<IEnumerable<string?>?> GetSpecialities(Degree degree, string major, CancellationToken cancellationToken);
}
