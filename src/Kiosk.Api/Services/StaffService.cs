using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Staff;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;
using MongoDB.Driver;

namespace KioskAPI.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    
    public StaffService(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }
    
    private static ProjectionDefinition<Academic> GetLanguageProjection(Language language)
    {
        return language switch
        {
            Language.Pl => Builders<Academic>.Projection.Exclude("en"),
            Language.En => Builders<Academic>.Projection.Exclude("pl"),
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }
    
    public async Task<IEnumerable<AcademicResponse>> GetStaff(Language language, CancellationToken cancellationToken)
    {
        var projection = GetLanguageProjection(language);
        var staff = await _staffRepository.GetStaff(projection, cancellationToken);
        return staff.Select(a => new AcademicResponse
        {
            _id = a._id,
            Name = a.Name,
            Link = a.Link,
            Email = a.Email,
            Content = a[language.ToString()],
            Language = language
        });
    }
    
    public async Task<AcademicResponse?> GetStaffMember(string academicId, Language language, CancellationToken cancellationToken)
    {
        var projection = GetLanguageProjection(language);
        var academic = await _staffRepository.GetAcademic(academicId, projection, cancellationToken);
        return academic is null ? null : new AcademicResponse
        {
            _id = academic._id,
            Name = academic.Name,
            Link = academic.Link,
            Email = academic.Email,
            Content = academic[language.ToString()],
            Language = language
        };
    }
}