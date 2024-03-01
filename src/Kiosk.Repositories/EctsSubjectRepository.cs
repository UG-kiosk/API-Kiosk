using Kiosk.Abstractions.Models;
using Kiosk.Repositories.Interfaces;
using MongoDB.Driver;

namespace Kiosk.Repositories;

public class EctsSubjectRepository : IEctsSubjectRepository
{
    private readonly string _collectionName = "ectsSubject";
    private readonly IMongoCollection<EctsSubject> _ectsSubjects;

    public EctsSubjectRepository(IMongoDatabase mongoDatabase)
    {
        _ectsSubjects = mongoDatabase.GetCollection<EctsSubject>(_collectionName);
    }

    public async Task<IEnumerable<EctsSubject>> GetEctsSubjects(CancellationToken cancellationToken)
       => (await _ectsSubjects.FindAsync(_ => true, cancellationToken: cancellationToken)).ToEnumerable();

    public async Task<IEnumerable<EctsSubject>?> GetEctsSubjectsByName(string subject,
        CancellationToken cancellationToken)
        => (await _ectsSubjects.FindAsync(r => r.Subject == subject, cancellationToken: cancellationToken))
            .ToEnumerable();

    public async Task CreateEctsSubject(EctsSubject ectsSubject, CancellationToken cancellationToken)
        => await _ectsSubjects.InsertOneAsync(ectsSubject, cancellationToken: cancellationToken);

    public async Task<EctsSubject?> DeleteEctsSubject(string id, CancellationToken cancellationToken) 
        =>  await _ectsSubjects.FindOneAndDeleteAsync(r => r._id == id, cancellationToken: cancellationToken);

    public async Task<EctsSubject?> UpdateEctsSubject(EctsSubject ectsSubject, CancellationToken cancellationToken)
    {
        var filter = Builders<EctsSubject>.Filter.Eq(r => r._id, ectsSubject._id);
        var updatedDocument =
            await _ectsSubjects.FindOneAndReplaceAsync(filter, ectsSubject, cancellationToken: cancellationToken);

        return updatedDocument;
    }
}