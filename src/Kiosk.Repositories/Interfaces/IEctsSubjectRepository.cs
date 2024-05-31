using System.Collections;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Pagination;

namespace Kiosk.Repositories.Interfaces;

public interface IEctsSubjectRepository
{
    Task CreateEctsSubject(EctsSubjectDocument ectsSubjectDocument, CancellationToken cancellationToken);

    Task<EctsSubjectDocument?> DeleteEctsSubject(string id, CancellationToken cancellationToken);

    Task<EctsSubjectDocument?> UpdateEctsSubject(EctsSubjectDocument ectsSubjectDocument, CancellationToken cancellationToken);
    
    Task<EctsSubjectDocument?> GetEctsSubjectsById(string id, CancellationToken cancellationToken);
    
    Task<IEnumerable<EctsSubjectDocument>?> GetEctsSubjectsByName(string subject, Language language, CancellationToken cancellationToken);

    Task<IEnumerable<string>?> GetMajors(Degree degree, Language language, CancellationToken cancellationToken);

    Task<IEnumerable<EctsSubjectDocument>?> GetSubjectsByMajor(EctsSubjectRequest ectsSubject, CancellationToken cancellationToken);

    Task<(IEnumerable<EctsSubjectDocument>, Pagination pagination)> GetEctsByPagination(PaginationRequest pagination, CancellationToken cancellationToken);

    Task<IEnumerable<int>?> GetYears(BaseEctsSubjectRequest baseEctsSubjectRequest, CancellationToken cancellationToken);

    Task<IEnumerable<string?>?> GetSpecialities(Degree degree, string major, Language language, CancellationToken cancellationToken);
}
