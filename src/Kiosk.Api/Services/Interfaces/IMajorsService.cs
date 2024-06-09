using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Major;

namespace KioskAPI.Services.Interfaces;

public interface IMajorsService
{
    Task<MajorResponse?> GetTranslatedMajor(string majorId, Language language, CancellationToken cancellationToken);
    Task<IEnumerable<MajorResponse>> GetTranslatedMajors(FindMajorsRequest findMajorsRequest, CancellationToken cancellationToken);
    Task CreateMajors(IEnumerable<CreateMajorRequest> createMajorsRequest, CancellationToken cancellationToken);
    Task<MajorResponse?> UpdateMajor(string id, CreateMajorRequest updateMajorRequest, CancellationToken cancellationToken);
}