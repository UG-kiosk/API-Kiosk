using System.Collections.Immutable;
using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Major;
using Kiosk.Abstractions.Models.Translation;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;
using MongoDB.Driver.Linq;

namespace KioskAPI.Services;

public class MajorsService : IMajorsService
{
    private readonly IMajorsRepository _majorsRepository;
    private readonly IMapper _mapper;
    private readonly ITranslatorService _translatorService;

    public MajorsService(IMajorsRepository majorsRepository, IMapper mapper, ITranslatorService translatorService)
    {
        _majorsRepository = majorsRepository;
        _mapper = mapper;
        _translatorService = translatorService;
    }
    
    private MajorResponse MapTranslatedMajor(MajorDocument majorDocument, Language language)
    {
        var mappedMajor = _mapper.Map<MajorResponse>(majorDocument);
        mappedMajor.Language = language;

        switch (language)
        {
            case Language.Pl:
                mappedMajor.Name = majorDocument.Pl.Name;
                mappedMajor.Content = majorDocument.Pl.Content;
                break;
            case Language.En:
                mappedMajor.Name = majorDocument.En.Name;
                mappedMajor.Content = majorDocument.En.Content;
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

    public async Task CreateMajor(IEnumerable<CreateMajorRequest> createMajorsRequest,
        CancellationToken cancellationToken)
    {
        var (nativeLanguageMajors, translationResults) = await TranslateMajors(createMajorsRequest, cancellationToken);

        var mappedMajors = MapTranslatedMajorsWithNative(nativeLanguageMajors, translationResults, createMajorsRequest);
        
        await _majorsRepository.CreateMajors(mappedMajors, cancellationToken);
    }

    private IEnumerable<MajorDocument> MapTranslatedMajorsWithNative(
        List<TranslationRequest<MajorDetails>> nativeLanguageMajors,
        IEnumerable<TranslationResponse<MajorDetails>>[] translationResults,
        IEnumerable<CreateMajorRequest> createMajorsRequest
    )
    {
        var mappedMajors = new List<MajorDocument>();
        
        foreach (var languageTranslationGroup in translationResults)
        {
            foreach (var translatedMajor in languageTranslationGroup)
            {
                var nativeLanguageMajor = nativeLanguageMajors.FirstOrDefault(
                    m => m.UniqueKey == translatedMajor.UniqueKey);
                var createMajorDto = createMajorsRequest.FirstOrDefault(
                    m => m.MajorDetails.Name == nativeLanguageMajor.TranslationPayload.Name);
                
                var majorDocument = new MajorDocument
                {
                    Pl = createMajorDto.MajorDetails,
                    En = translatedMajor.Translations[Language.En],
                    Degree = createMajorDto.Degree,
                    Url = createMajorDto.Url
                };
                
                mappedMajors.Add(majorDocument);
            }
        }

        return mappedMajors;
    }
    
    private async Task<(
        List<TranslationRequest<MajorDetails>> nativeLanguageMajors,
        IEnumerable<TranslationResponse<MajorDetails>>[] translationResults
        )> TranslateMajors(
        IEnumerable<CreateMajorRequest> createMajorsRequest, 
        CancellationToken cancellationToken)
    {
        var groupedByLanguage = createMajorsRequest.GroupBy(request => request.SourceLanguage);

        var nativeLanguageMajors = new List<TranslationRequest<MajorDetails>>();
        var translatedMajors = new List<Task<IEnumerable<TranslationResponse<MajorDetails>>>>();

        ImmutableList<Language> supportedLanguages = ImmutableList.Create(Language.En, Language.Pl);

        foreach (var majorLanguageGroup in groupedByLanguage)
        {
            var translationContent = majorLanguageGroup.Select(majorGroup => new TranslationRequest<MajorDetails>
            {
                UniqueKey = Guid.NewGuid().ToString(),
                TranslationPayload = majorGroup.MajorDetails
            }).ToList();
            nativeLanguageMajors.AddRange(translationContent);

            var translationTask = _translatorService.Translate(
                translationContent,
                majorLanguageGroup.Key,
                supportedLanguages.Where(language => language != majorLanguageGroup.Key),
                cancellationToken);

            translatedMajors.Add(translationTask);
        }
        
        var translationResults = await Task.WhenAll(translatedMajors);
    
        return (nativeLanguageMajors, translationResults);
    }
}