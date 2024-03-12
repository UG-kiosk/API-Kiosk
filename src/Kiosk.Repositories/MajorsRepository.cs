using System.Text.RegularExpressions;
using Kiosk.Abstractions.Dtos;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
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
    
    public async Task<IEnumerable<Major>> GetMajors(FindMajorsQueryDto findMajorsQueryDto, CancellationToken cancellationToken)
    {
        var languageField = findMajorsQueryDto.Language == Language.PL ? "pl" : "en";
        
        var filter = Builders<Major>.Filter.Empty;
        
        
        if (findMajorsQueryDto.Degree != null)
        {
            var degreeString = Enum.GetName(typeof(Degree), findMajorsQueryDto.Degree);

            filter &= Builders<Major>.Filter.Eq("degree", degreeString);   
        }
        
        if (findMajorsQueryDto.Name != null)
        {
            filter &= Builders<Major>.Filter.Regex($"{languageField}.name", 
                new BsonRegularExpression(new Regex(findMajorsQueryDto.Name, RegexOptions.IgnoreCase)));
        }
        
        return await _majors.Find(filter)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Major?> DeleteMajor(string id, CancellationToken cancellationToken)
        => await _majors.FindOneAndDeleteAsync(major => major._id == id, cancellationToken: cancellationToken);
}
