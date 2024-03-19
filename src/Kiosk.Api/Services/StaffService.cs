using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Staff;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;

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
    
    public async Task<(IEnumerable<AcademicToListResponse>, Pagination Pagination)> GetStaff(Language language, 
        int? page, 
        int? itemsPerPage, 
        string? name, 
        CancellationToken cancellationToken)
    {
        var pagination = new Pagination
        {
            Page = page.GetValueOrDefault(1),
            ItemsPerPage = itemsPerPage.GetValueOrDefault(30)
        };
    
        var (staff, updatedPagination) = await _staffRepository.GetStaff(language, pagination,
            name, cancellationToken);
        var staffResponse = _mapper.Map<List<AcademicToListResponse>>(staff, opt => opt.Items["language"] = language);
        
        return (staffResponse, updatedPagination);
    }
    
    public async Task<AcademicResponse?> GetStaffMember(string academicId, Language language, CancellationToken cancellationToken)
    {
        var academic = await _staffRepository.GetAcademic(academicId, language, cancellationToken);
        return academic is null ? null : _mapper.Map<AcademicResponse>(academic, opt => opt.Items["language"] = language);
    }
}