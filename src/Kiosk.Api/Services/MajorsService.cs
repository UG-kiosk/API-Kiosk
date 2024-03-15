using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Major;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class MajorsService : IMajorsService
{
    private readonly IMajorsRepository _majorsRepository;
    private readonly IMapper _mapper;

    public MajorsService(IMajorsRepository majorsRepository, IMapper mapper)
    {
        _majorsRepository = majorsRepository;
        _mapper = mapper;
    }
    
    // maybe it could be done with auto mapper
    private MajorResponse MapTranslatedMajor(Major major, Language language)
    {
        var mappedMajor = _mapper.Map<MajorResponse>(major);
        mappedMajor.Language = language;

        switch (language)
        {
            case Language.Pl:
                mappedMajor.Name = major.Pl.Name;
                mappedMajor.Content = major.Pl.Content;
                break;
            case Language.En:
                mappedMajor.Name = major.En.Name;
                mappedMajor.Content = major.En.Content;
                break;
        }
        
        return mappedMajor;
    }

    public async Task<MajorResponse?> GetTranslatedMajor(string majorId, Language language, CancellationToken cancellationToken)
    {
        var major = await _majorsRepository.GetMajor(majorId, cancellationToken);
        
        if (major is null)
        {
            return null;
        }

        return MapTranslatedMajor(major, language);
    }

    public async Task<IEnumerable<MajorResponse>> GetTranslatedMajors(FindMajorsRequest findMajorsRequest, CancellationToken cancellationToken)
    {
        var majorsList = await _majorsRepository.GetMajors(findMajorsRequest, cancellationToken);
    
        return majorsList.Select(major => MapTranslatedMajor(major, findMajorsRequest.Language));   
    }
}