using Kiosk.Abstractions.Dtos;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;

namespace KioskAPI.Services.Interfaces;

public interface IMajorsService
{
    Task<MajorOutputDto?> GetTranslatedMajor(string majorId, Language language, CancellationToken cancellationToken);
    Task<IEnumerable<MajorOutputDto>> GetTranslatedMajors(FindMajorsQueryDto findMajorsQueryDto,
        CancellationToken cancellationToken);
}