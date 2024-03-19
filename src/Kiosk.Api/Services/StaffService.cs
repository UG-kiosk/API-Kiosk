using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Staff;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;
using MongoDB.Driver;

namespace KioskAPI.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IMapper _mapper;
    
    public StaffService(IStaffRepository staffRepository, IMapper mapper)
    {
        _staffRepository = staffRepository;
        _mapper = mapper;
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
    
    public async Task<(IEnumerable<AcademicToListResponse>, Pagination Pagination)> GetStaff(Language language, int? page, 
        int? itemsPerPage, string? name, CancellationToken cancellationToken)
    {
        var projection = GetLanguageProjection(language);
        projection = projection.Exclude(a => a.Email);
        var pagination = new Pagination
        {
            Page = page.GetValueOrDefault(1),
            ItemsPerPage = itemsPerPage.GetValueOrDefault(30)
        };
    
        var (staff, updatedPagination) = await _staffRepository.GetStaff(projection, pagination,
            name, cancellationToken);
        var staffResponse = _mapper.Map<List<AcademicToListResponse>>(staff, opt => opt.Items["language"] = language);
        
        return (staffResponse, updatedPagination);
    }
    
    public async Task<AcademicResponse?> GetStaffMember(string academicId, Language language, 
        CancellationToken cancellationToken)
    {
        var projection = GetLanguageProjection(language);
        var academic = await _staffRepository.GetAcademic(academicId, projection, cancellationToken);
        return academic is null ? null : _mapper.Map<AcademicResponse>(academic, opt => opt.Items["language"] = language);
    }
}