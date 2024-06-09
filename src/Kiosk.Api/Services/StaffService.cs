using System.Collections.Immutable;
using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.Staff;
using Kiosk.Abstractions.Models.Translation;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IMapper _mapper;
    private readonly ITranslatorService _translatorService;

    public StaffService(IStaffRepository staffRepository, IMapper mapper, ITranslatorService translatorService)
    {
        _staffRepository = staffRepository;
        _mapper = mapper;
        _translatorService = translatorService;
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
    
    public async Task CreateStaff(IEnumerable<AcademicRequest> staff, CancellationToken cancellationToken)
    {
        var mappedStaff = await TranslateStaff(staff, cancellationToken);
        await _staffRepository.CreateStaff(mappedStaff, cancellationToken);
    }
    
    public async Task CreateOrReplaceStaff(IEnumerable<AcademicRequest> staff, CancellationToken cancellationToken)
    {
        var mappedStaff = await TranslateStaff(staff, cancellationToken);
        await _staffRepository.CreateOrReplaceStaff(mappedStaff, cancellationToken);
    }
    
    public async Task<AcademicResponse?> UpdateStaffMember(string academicId, AcademicRequest academic, CancellationToken cancellationToken)
    {
        var translatedStaff = await TranslateStaff(new List<AcademicRequest> { academic }, cancellationToken);
        var updatedStaffMember = await _staffRepository.UpdateStaffMember(academicId, translatedStaff.First(), cancellationToken);

        return _mapper.Map<AcademicResponse>(updatedStaffMember, opt => opt.Items["language"] = Language.Pl);
    }
    
    private async Task<IEnumerable<Academic>> TranslateStaff(IEnumerable<AcademicRequest> staff, CancellationToken cancellationToken)
    {
        ImmutableList<Language> supportedLanguages = new List<Language> { Language.En, Language.Pl }.ToImmutableList();

        var simplifiedStaff = staff.Select(staffGroup => _mapper.Map<SimplifiedAcademic>(staffGroup)).ToList();
        
        var translationContent = simplifiedStaff.Select(staffGroup => new TranslationRequest<AcademicSimplifiedContent>
        {
            UniqueKey = Guid.NewGuid().ToString(),
            TranslationPayload = staffGroup.Content
        }).ToList();
        
        var translationTask = await _translatorService.Translate(
            translationContent,
            Language.Pl,
            supportedLanguages.Where(language => language != Language.Pl),
            cancellationToken);
        
        var translatedStaff = translationTask.Select(translatedContent =>
        {
            var originalContent = simplifiedStaff.First(m =>
                m.Content == translationContent.First(n => n.UniqueKey == translatedContent.UniqueKey).TranslationPayload);
            return new Academic
            {
                Name = originalContent.Name,
                Link = originalContent.Link,
                Email = originalContent.Email,
                Pl = _mapper.Map<AcademicContent>(originalContent.Content),
                En = _mapper.Map<AcademicContent>(translatedContent.Translations[Language.En]),
            };
        });
        return translatedStaff;
    }
}
