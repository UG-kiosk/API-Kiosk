using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Pagination;

namespace KioskAPI.Services.Interfaces;

public interface IEctsSubjectService
{
    Task<(IEnumerable<EctsSubjectCreateRequest>, Pagination Pagination)> GetEcts(PaginationRequest paginationRequest, CancellationToken cancellationToken);
    
    Task<EctsSubjectCreateRequest> GetOneEcts(string id, CancellationToken cancellationToken);
    
    Task<EctsSubjectDocument?> UpdateEctsSubject(EctsSubjectCreateRequest ectsSubjectDocument, CancellationToken cancellationToken);
    
    Task<bool> AddEctsSubject(EctsSubjectCreateRequest ectsSubjectDocument, CancellationToken cancellationToken);
    
    Task<EctsSubjectResponse> GetSubjectsByMajor(EctsSubjectRequest ectsSubjectRequest, CancellationToken cancellationToken);

    Task<IEnumerable<string>?> GetMajorsOrSpecialities(Degree degree, Language language, string? major, CancellationToken cancellationToken);

    Task<IEnumerable<int>?> GetYears(BaseEctsSubjectRequest baseEctsSubjectRequest,
        CancellationToken cancellationToken);
}