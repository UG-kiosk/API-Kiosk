using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;
using ILogger = Serilog.ILogger;


namespace KioskAPI.Services;

public class EctsSubjectService : IEctsSubjectService
{
    private readonly IEctsSubjectRepository _ectsSubjectRepository;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;


    public EctsSubjectService(IEctsSubjectRepository ectsSubjectRepository, ILogger logger, IMapper mapper)
    {
        _ectsSubjectRepository = ectsSubjectRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> AddEctsSubject(EctsSubjectDocument ectsSubjectDocument, CancellationToken cancellationToken)
    {
        var existingDocument = await _ectsSubjectRepository.GetEctsSubjectsByName(ectsSubjectDocument.Subject, cancellationToken);

        bool isTheSameEctsSubject(EctsSubjectDocument document) =>
            document.Subject == ectsSubjectDocument.Subject && document.Major == ectsSubjectDocument.Major && document.Degree == ectsSubjectDocument.Degree;

        if (existingDocument.Any(x => isTheSameEctsSubject(x))) return false;

        await _ectsSubjectRepository.CreateEctsSubject(ectsSubjectDocument, cancellationToken);

        return true;
    }
    
    public async Task<IEnumerable<string>?> GetMajorsOrSpecialities(Degree degree, string? major, CancellationToken cancellationToken)
    {
        var result = major is null
            ? await _ectsSubjectRepository.GetMajors(degree, cancellationToken)
            : await _ectsSubjectRepository.GetSpecialities(degree, major, cancellationToken);
        
        return result;
    }
    
    

    public async Task<EctsSubjectResponse?> GetSubjectsByMajor(EctsSubjectRequest ectsSubjectRequest, CancellationToken cancellationToken)
    {
        var ectsSubjectDocuments = (await _ectsSubjectRepository.GetSubjectsByMajor(ectsSubjectRequest, cancellationToken)).ToList();
        
        EctsSubjectResponse ectsSubjectResponse = new EctsSubjectResponse()
        {
            Degree = ectsSubjectRequest.Degree,
            Major = ectsSubjectRequest.Major ?? ectsSubjectDocuments.First().Speciality,
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
                Subjects = x.Select(subject => _mapper.Map<SubjectResponse>(subject)).OrderBy(x=>x.Subject)
            };

            var sum = new SubjectResponse()
            {
                Subject = "Razem",
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
}