using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;

namespace KioskAPI.Services.Interfaces;

public interface IEctsSubjectService
{
    Task<bool> AddEctsSubject(EctsSubjectDocument ectsSubjectDocument, CancellationToken cancellationToken);
    
    Task<EctsSubjectResponse> GetSubjectsByMajor(EctsSubjectRequest ectsSubjectRequest, CancellationToken cancellationToken);

    Task<IEnumerable<string>?> GetMajorsOrSpecialities(Degree degree, string? major, CancellationToken cancellationToken);
}