using Kiosk.Abstractions.Models;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class EctsSubjectService : IEctsSubjectService
{
    private readonly IEctsSubjectRepository _ectsSubjectRepository;

    public EctsSubjectService(IEctsSubjectRepository ectsSubjectRepository)
    {
        _ectsSubjectRepository = ectsSubjectRepository;
    }

    public async Task<bool> AddEctsSubject(EctsSubject ectsSubject, CancellationToken cancellationToken)
    {
        var result = await _ectsSubjectRepository.GetEctsSubjectsByName(ectsSubject.Subject, cancellationToken);

        Predicate<EctsSubject> isTheSameEctsSubject = x =>
            x.Subject == ectsSubject.Subject && x.Major == ectsSubject.Major && x.Degree == ectsSubject.Degree;

        if (result is null || result.Any(x => isTheSameEctsSubject(x))) return false;

        await _ectsSubjectRepository.CreateEctsSubject(ectsSubject, cancellationToken);

        return true;
    }
}