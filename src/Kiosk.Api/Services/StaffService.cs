using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Staff;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;
using MongoDB.Bson;
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
    
    public async Task<(IEnumerable<AcademicResponse>, Pagination Pagination)> GetStaff(Language language, 
        CancellationToken cancellationToken, int page, int itemsPerPage, string name)
    {
        var projection = GetLanguageProjection(language);
        var filterBuilder = Builders<Academic>.Filter;
        var filter = !string.IsNullOrEmpty(name) 
            ? filterBuilder.Regex(a => a.Name, new BsonRegularExpression(name, "i")) 
            : filterBuilder.Empty;
        
        var pagination = new Pagination
        {
            Page = page,
            ItemsPerPage = itemsPerPage
        };
    
        var (staff, updatedPagination) = await _staffRepository.GetStaff(projection, pagination,
            filter, cancellationToken);
        var staffResponse = staff.Select(a => new AcademicResponse
        {
            _id = a._id,
            Name = a.Name,
            Link = a.Link,
            Email = a.Email,
            Content = a[language.ToString()],
            Language = language
        }).ToList();

        return (staffResponse, updatedPagination);
    }
    
    public async Task<AcademicResponse?> GetStaffMember(string academicId, Language language, 
        CancellationToken cancellationToken)
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