using System.Text.RegularExpressions;
using Kiosk.Abstractions.Models.Major;
using Kiosk.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kiosk.Repositories;

public class MajorsRepository : IMajorsRepository
{
    private readonly string _collectionName = "majors";
    private readonly IMongoCollection<MajorDocument> _majors;
    
    public MajorsRepository(IMongoDatabase mongoDatabase)
    {
        _majors = mongoDatabase.GetCollection<MajorDocument>(_collectionName);
    }

    public async Task<MajorDocument?> GetMajor(string id, CancellationToken cancellationToken)
        => await _majors.Find(major => major._id == id)
            .FirstOrDefaultAsync(cancellationToken);
    
    public async Task<IEnumerable<MajorDocument>> GetMajors(FindMajorsRequest findMajorsRequest, CancellationToken cancellationToken)
    {
        var filter = Builders<MajorDocument>.Filter.Empty;
        
        if (findMajorsRequest.Degree != null)
        {
            filter &= Builders<MajorDocument>.Filter.Eq(major => major.Degree, findMajorsRequest.Degree);
        }
        
        if (findMajorsRequest.Name != null)
        {
            filter &= Builders<MajorDocument>.Filter.Regex($"{findMajorsRequest.Language.ToString()}.name", 
                new BsonRegularExpression(new Regex(findMajorsRequest.Name, RegexOptions.IgnoreCase)));
        }
        
        return await _majors.Find(filter)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<MajorDocument?> DeleteMajor(string id, CancellationToken cancellationToken)
        => await _majors.FindOneAndDeleteAsync(major => major._id == id, cancellationToken: cancellationToken);
    
    public async Task CreateMajor(MajorDocument majorDocument, CancellationToken cancellationToken) =>
        await _majors.InsertOneAsync(majorDocument, cancellationToken: cancellationToken);
    
    public async Task CreateMajors(IEnumerable<MajorDocument> majorDocuments, CancellationToken cancellationToken)
    {
        await _majors.InsertManyAsync(majorDocuments, cancellationToken: cancellationToken);
    }
}
