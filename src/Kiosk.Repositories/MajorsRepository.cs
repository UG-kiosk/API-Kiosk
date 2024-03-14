using System.Text.RegularExpressions;
using Kiosk.Abstractions.Models.Major;
using Kiosk.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kiosk.Repositories;

public class MajorsRepository : IMajorsRepository
{
    private readonly string _collectionName = "majors";
    private readonly IMongoCollection<Major> _majors;
    
    public MajorsRepository(IMongoDatabase mongoDatabase)
    {
        _majors = mongoDatabase.GetCollection<Major>(_collectionName);
    }

    public async Task<Major?> GetMajor(string id, CancellationToken cancellationToken)
        => await _majors.Find(major => major._id == id)
            .FirstOrDefaultAsync(cancellationToken);
    
    public async Task<IEnumerable<Major>> GetMajors(FindMajorsRequest findMajorsRequest, CancellationToken cancellationToken)
    {
        var filter = Builders<Major>.Filter.Empty;
        
        if (findMajorsRequest.Degree != null)
        {
            filter &= Builders<Major>.Filter.Eq(major => major.Degree, findMajorsRequest.Degree);
        }
        
        if (findMajorsRequest.Name != null)
        {
            filter &= Builders<Major>.Filter.Regex($"{findMajorsRequest.Language.ToString()}.name", 
                new BsonRegularExpression(new Regex(findMajorsRequest.Name, RegexOptions.IgnoreCase)));
        }
        
        return await _majors.Find(filter)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Major?> DeleteMajor(string id, CancellationToken cancellationToken)
        => await _majors.FindOneAndDeleteAsync(major => major._id == id, cancellationToken: cancellationToken);
}
