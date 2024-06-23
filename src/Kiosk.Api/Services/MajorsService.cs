using System.Collections.Immutable;
using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Major;
using Kiosk.Abstractions.Models.Translation;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;

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

    public async Task CreateMajors(IEnumerable<CreateMajorRequest> createMajorsRequest,
        CancellationToken cancellationToken)
    {
        var mappedMajors = await TranslateMajors(createMajorsRequest, cancellationToken);
        await _majorsRepository.CreateMajors(mappedMajors, cancellationToken);
    }

    public async Task<MajorResponse?> UpdateMajor(string id, CreateMajorRequest updateMajorRequest, CancellationToken cancellationToken)
    {
        var translatedMajors = await TranslateMajors(new List<CreateMajorRequest> { updateMajorRequest }, cancellationToken);
        var updatedMajors = await _majorsRepository.UpdateMajor(id,translatedMajors.First(), cancellationToken);
        return _mapper.Map<MajorResponse>(updatedMajors);
    }

    private async Task<IEnumerable<MajorDocument>> TranslateMajors(
        IEnumerable<CreateMajorRequest> createMajorsRequest,
        CancellationToken cancellationToken)
    {
        var groupedByLanguage = createMajorsRequest.GroupBy(request => request.SourceLanguage);
        ImmutableList<Language> supportedLanguages = new List<Language> { Language.En, Language.Pl }.ToImmutableList();

        var translationTasks = groupedByLanguage.Select(async majorLanguageGroup =>
        {
            var translationContent = majorLanguageGroup.Select(majorGroup => new TranslationRequest<MajorDetails>
            {
                UniqueKey = Guid.NewGuid().ToString(),
                TranslationPayload = majorGroup.MajorDetails
            }).ToList();

            var translationTask = await _translatorService.Translate(
                translationContent,
                majorLanguageGroup.Key,
                supportedLanguages.Where(language => language != majorLanguageGroup.Key),
                cancellationToken);

            return translationTask.Select(translatedMajor =>
            {
                var nativeLanguageMajor = translationContent.FirstOrDefault(
                    m => m.UniqueKey == translatedMajor.UniqueKey);
                var createMajorDto = createMajorsRequest.FirstOrDefault(
                    m => m.MajorDetails.Name == nativeLanguageMajor!.TranslationPayload.Name);

                return new MajorDocument
                {
                    Pl = createMajorDto!.SourceLanguage == Language.Pl ?
                        createMajorDto.MajorDetails : translatedMajor.Translations[Language.Pl],
                    En = createMajorDto.SourceLanguage == Language.En ?
                        createMajorDto.MajorDetails : translatedMajor.Translations[Language.En],
                    Degree = createMajorDto.Degree,
                    Url = createMajorDto.Url
                };
            });
        });

        var translationResults = await Task.WhenAll(translationTasks);
        return translationResults.SelectMany(x => x);
    }
}