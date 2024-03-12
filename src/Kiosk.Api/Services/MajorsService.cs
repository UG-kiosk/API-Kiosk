using AutoMapper;
using Kiosk.Abstractions.Dtos;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
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
    
    private MajorOutputDto MapTranslatedMajor(Major major, Language language)
    {
        var mappedMajor = _mapper.Map<MajorOutputDto>(major);
        mappedMajor.Language = language;

        switch (language)
        {
            case Language.PL:
                mappedMajor.Name = major.PolishDetails.Name;
                mappedMajor.Content = major.PolishDetails.Content;
                break;
            case Language.EN:
                mappedMajor.Name = major.EnglishDetails.Name;
                mappedMajor.Content = major.EnglishDetails.Content;
                break;
        }
        
        return mappedMajor;
    }

    public async Task<MajorOutputDto?> GetTranslatedMajor(string majorId, Language language, CancellationToken cancellationToken)
    {
        var major = await _majorsRepository.GetMajor(majorId, cancellationToken);
        
        if (major is null)
        {
            return null;
        }

        return MapTranslatedMajor(major, language);
    }

    public async Task<IEnumerable<MajorOutputDto>> GetTranslatedMajors(FindMajorsQueryDto findMajorsQueryDto, CancellationToken cancellationToken)
    {
        var majorsList = await _majorsRepository.GetMajors(findMajorsQueryDto, cancellationToken);
    
        return majorsList.Select(major => MapTranslatedMajor(major, findMajorsQueryDto.Language));   
    }
}