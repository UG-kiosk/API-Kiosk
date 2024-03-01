using Kiosk.Abstractions.Models;

namespace KioskAPI.Services.Interfaces;

public interface IEctsSubjectService
{
    Task<bool> AddEctsSubject(EctsSubject ectsSubject, CancellationToken cancellationToken);
}