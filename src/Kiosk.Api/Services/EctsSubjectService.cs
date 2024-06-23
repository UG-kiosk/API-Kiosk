using System.Collections.Immutable;
using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Major;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.Translation;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;
using ILogger = Serilog.ILogger;


namespace KioskAPI.Services;

public class EctsSubjectService : IEctsSubjectService
{
    private readonly IEctsSubjectRepository _ectsSubjectRepository;
    private readonly ITranslatorService _translatorService;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;


    public EctsSubjectService(IEctsSubjectRepository ectsSubjectRepository, ILogger logger, IMapper mapper, ITranslatorService translatorService)
    {
        _ectsSubjectRepository = ectsSubjectRepository;
        _logger = logger;
        _mapper = mapper;
        _translatorService = translatorService;
    }

    public async Task<bool> AddEctsSubject(EctsSubjectCreateRequest ectsSubjectDocument, CancellationToken cancellationToken)
    {
        var existingDocument = await _ectsSubjectRepository.GetEctsSubjectsByName(ectsSubjectDocument.Subject, Language.Pl, cancellationToken);

        bool isTheSameEctsSubject(EctsSubjectDocument document) =>
            document.Pl.Subject == ectsSubjectDocument.Subject && document.Pl.Major == ectsSubjectDocument.Major && document.Degree == ectsSubjectDocument.Degree;

        if (existingDocument.Any(x => isTheSameEctsSubject(x))) return false;



        var translatedDocument = (await GetTranslatedEcts(new[] { ectsSubjectDocument }, Language.Pl, cancellationToken)).FirstOrDefault();
        

        await _ectsSubjectRepository.CreateEctsSubject(translatedDocument, cancellationToken);

        return true;
    }

    public async Task<(IEnumerable<EctsSubjectCreateRequest>, Pagination Pagination)> GetEcts(PaginationRequest paginationRequest, CancellationToken cancellationToken)
    {

        var (ectsSubjectDocuments, paginationResponse) =await _ectsSubjectRepository.GetEctsByPagination(paginationRequest, cancellationToken);

        var ectsResponse = ectsSubjectDocuments.Select(subject =>
            _mapper.Map<EctsSubjectCreateRequest>(subject, opt => opt.Items["language"] = Language.Pl));


        return (ectsResponse, paginationResponse);
    }
    
    public async Task<EctsSubjectCreateRequest> GetOneEcts(string id, CancellationToken cancellationToken)
    {
        var ectsSubjectDocument = await _ectsSubjectRepository.GetEctsSubjectsById(id, cancellationToken);

        var ectsResponse = new EctsSubjectCreateRequest()
        {
            Degree = ectsSubjectDocument.Degree,
            Ects = ectsSubjectDocument.Ects,
            Major = ectsSubjectDocument.Pl.Major,
            Subject = ectsSubjectDocument.Pl.Subject,
            Term = ectsSubjectDocument.Term,
            Year = ectsSubjectDocument.Year,
            LabsHours = ectsSubjectDocument.LabsHours,
            LectureHours = ectsSubjectDocument.LectureHours,
            RecitationHours = ectsSubjectDocument.RecitationHours,
            RecruitmentYear = ectsSubjectDocument.RecruitmentYear,
            Pass = ectsSubjectDocument.Pass,
            Speciality = ectsSubjectDocument.Pl.Speciality,
            _id = ectsSubjectDocument._id
        };
        
        return ectsResponse;
    }
    
    public async Task<EctsSubjectDocument?> UpdateEctsSubject(EctsSubjectCreateRequest ectsSubjectDocument, CancellationToken cancellationToken)
    {
        var translatedDocument = (await GetTranslatedEcts(new[] { ectsSubjectDocument }, Language.Pl, cancellationToken)).FirstOrDefault();
        
        var existingDocument = await _ectsSubjectRepository.UpdateEctsSubject(translatedDocument, cancellationToken);


        return existingDocument;
    }
    
    public async Task<IEnumerable<string>?> GetMajorsOrSpecialities(Degree degree, Language language, string? major, CancellationToken cancellationToken)
    {
        var result = major is null
            ? await _ectsSubjectRepository.GetMajors(degree,language, cancellationToken)
            : await _ectsSubjectRepository.GetSpecialities(degree, major,language, cancellationToken);
        
        return result;
    }
    
    

    public async Task<EctsSubjectResponse?> GetSubjectsByMajor(EctsSubjectRequest ectsSubjectRequest, CancellationToken cancellationToken)
    {
        var ectsSubjectDocuments = (await _ectsSubjectRepository.GetSubjectsByMajor(ectsSubjectRequest, cancellationToken)).ToList();
        
        EctsSubjectResponse ectsSubjectResponse = new EctsSubjectResponse()
        {
            Degree = ectsSubjectRequest.Degree,
            Major = ectsSubjectRequest.Major ?? ectsSubjectDocuments.First()[ectsSubjectRequest.Language].Speciality,
            RecruitmentYear = ectsSubjectRequest.Year,
            SubjectsByYearAndTerm = new List<SubjectsByYearAndTerm>()
        };

        var groupedSubjects = ectsSubjectDocuments.GroupBy(e => new { e.Year, e.Term }).ToList();
        
        var transformedSubjects = groupedSubjects.Select(x =>
        {
            var subjectByYearAndTerm = new SubjectsByYearAndTerm()
            {
                Year = x.Key.Year,
                Term = x.Key.Term,
                Subjects = x.Select(subject => _mapper.Map<SubjectResponse>(subject,opt => opt.Items["language"] = ectsSubjectRequest.Language)).OrderBy(x=>x.Subject)
            };

            var sum = new SubjectResponse()
            {

                Major = "razem",
                Subject = "razem",
                Ects = x.Sum(x => x.Ects),
                LabsHours = x.Sum(x => x.LabsHours),
                LectureHours = x.Sum(x => x.LectureHours),
                RecitationHours = x.Sum(x => x.RecitationHours),
                Pass = ""
            };

            subjectByYearAndTerm.Subjects = subjectByYearAndTerm.Subjects.Append(sum);

            return subjectByYearAndTerm;
        });

        ectsSubjectResponse.SubjectsByYearAndTerm = transformedSubjects.OrderBy(x => x.Term);
        
        return ectsSubjectResponse;
    }
    
    public async Task<IEnumerable<int>?> GetYears(BaseEctsSubjectRequest baseEctsSubjectRequest, CancellationToken cancellationToken)
        => (await _ectsSubjectRepository.GetYears(baseEctsSubjectRequest, cancellationToken)).OrderDescending();

    private async Task<IEnumerable<EctsSubjectDocument>> GetTranslatedEcts (IEnumerable<EctsSubjectCreateRequest> notTranslatedResponses, Language sourceLanguage, CancellationToken cancellationToken)
    {
        ImmutableList<Language> supportedLanguages = new List<Language> { Language.En, Language.Pl }.ToImmutableList();
        
        var translationContent = notTranslatedResponses.Select(subject =>
        {
            var translationPayload = new EctsSubjectDetails()
            {
                Major = subject.Major,
                Subject = subject.Subject,
                Speciality = subject.Speciality ?? ""
            };

            var translationContent = new TranslationRequest<EctsSubjectDetails>()
            {
                UniqueKey = Guid.NewGuid().ToString(),
                TranslationPayload = translationPayload
            };
            
            return translationContent;
        }).ToList();
        
        var translated = await _translatorService.Translate(
            translationContent,
            sourceLanguage,
            supportedLanguages.Where(supLanguage => supLanguage != sourceLanguage),
            cancellationToken);
        
        return translated.Select(translatedDocument =>
        {
            var nativeDocument = translationContent.FirstOrDefault(
                m => m.UniqueKey == translatedDocument.UniqueKey);
            var createDocumentDto = notTranslatedResponses.FirstOrDefault(
                m => m.Subject == nativeDocument!.TranslationPayload.Subject);

            return new EctsSubjectDocument()
            {
                _id = createDocumentDto._id,
                Pl = sourceLanguage == Language.Pl ?
                    new EctsSubjectDetails()
                    {
                        Major = createDocumentDto.Major,
                        Subject = createDocumentDto.Subject,
                        Speciality = createDocumentDto.Speciality
                    } : translatedDocument.Translations[Language.Pl],
                En = sourceLanguage == Language.En ?
                    new EctsSubjectDetails()
                    {
                        Major = createDocumentDto.Major,
                        Subject = createDocumentDto.Subject,
                        Speciality = createDocumentDto.Speciality
                    } : translatedDocument.Translations[Language.En],
                Degree = createDocumentDto.Degree,
                Ects = createDocumentDto.Ects,
                Term = createDocumentDto.Term,
                Year = createDocumentDto.Year,
                LabsHours = createDocumentDto.LabsHours,
                LectureHours = createDocumentDto.LectureHours,
                RecitationHours = createDocumentDto.RecitationHours,
                RecruitmentYear = createDocumentDto.RecruitmentYear,
                Pass = createDocumentDto.Pass
            };
        });
    }


}