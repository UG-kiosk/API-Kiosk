using System.Collections;
using Kiosk.Abstractions.Models;
using Kiosk.Repositories.Interfaces;
using MongoDB.Driver;

namespace Kiosk.Repositories;

public class EctsSubjectRepository : IEctsSubjectRepository
{
    private static readonly string _collectionName = "ectsSubjects";
    private readonly IMongoCollection<EctsSubjectDocument> _ectsSubjects;

    public EctsSubjectRepository(IMongoDatabase mongoDatabase)
    {
        _ectsSubjects = mongoDatabase.GetCollection<EctsSubjectDocument>(_collectionName);
    }

    public async Task<IEnumerable<EctsSubjectDocument>> GetEctsSubjects(CancellationToken cancellationToken)
       => (await _ectsSubjects.FindAsync(_ => true, cancellationToken: cancellationToken)).ToEnumerable();

    public async Task<IEnumerable<EctsSubjectDocument>?> GetEctsSubjectsByName(string subject, CancellationToken cancellationToken)
        => (await _ectsSubjects.FindAsync(r => r.Subject == subject, cancellationToken: cancellationToken))
            .ToEnumerable();

    public async Task CreateEctsSubject(EctsSubjectDocument ectsSubjectDocument, CancellationToken cancellationToken)
        => await _ectsSubjects.InsertOneAsync(ectsSubjectDocument, cancellationToken: cancellationToken);

    public async Task<EctsSubjectDocument?> DeleteEctsSubject(string id, CancellationToken cancellationToken) 
        =>  await _ectsSubjects.FindOneAndDeleteAsync(r => r._id == id, cancellationToken: cancellationToken);

    public async Task<EctsSubjectDocument?> UpdateEctsSubject(EctsSubjectDocument ectsSubjectDocument, CancellationToken cancellationToken)
    {
        var filter = Builders<EctsSubjectDocument>.Filter.Eq(r => r._id, ectsSubjectDocument._id);
        
        var updatedDocument =
            await _ectsSubjects.FindOneAndReplaceAsync(filter, ectsSubjectDocument, cancellationToken: cancellationToken);

        return updatedDocument;
    }

    public async Task<IEnumerable<string>?> GetMajors(Degree degree, CancellationToken cancellationToken)
    => (await _ectsSubjects.DistinctAsync(subject => subject.Major, DegreeFilter(degree) , cancellationToken: cancellationToken)).ToEnumerable();
    
    public async Task<IEnumerable<EctsSubjectDocument>?> GetSubjectsByMajor(EctsSubjectRequest ectsSubject, CancellationToken cancellationToken)
    {
        var yearFilter = Builders<EctsSubjectDocument>.Filter.Where(x => x.RecruitmentYear.Contains(ectsSubject.Year));
        
        var updatedDocument = (await _ectsSubjects
            .FindAsync(MajorOrSpecialityFilter(ectsSubject.Major, ectsSubject.Speciality) & DegreeFilter(ectsSubject.Degree) & yearFilter, cancellationToken: cancellationToken))
            .ToEnumerable();
    
        return updatedDocument;
    }

    public async Task<IEnumerable<int>?> GetYears(BaseEctsSubjectRequest baseEctsSubjectRequest, CancellationToken cancellationToken) => 
        (await _ectsSubjects.DistinctAsync(e => e.RecruitmentYear.First(), MajorOrSpecialityFilter(baseEctsSubjectRequest.Major, baseEctsSubjectRequest.Speciality) & DegreeFilter(baseEctsSubjectRequest.Degree)))
        .ToList();

    
    private FilterDefinition<EctsSubjectDocument> MajorOrSpecialityFilter(string? major, string? speciality) =>
        Builders<EctsSubjectDocument>.Filter.Where(subject => speciality == null ? subject.Major == major : subject.Speciality == speciality);
    
    private FilterDefinition<EctsSubjectDocument> DegreeFilter(Degree degree) =>
        Builders<EctsSubjectDocument>.Filter.Eq(r => r.Degree, degree);
    
    
}