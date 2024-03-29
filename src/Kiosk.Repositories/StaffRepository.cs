using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.Staff;
using Kiosk.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kiosk.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly string _collectionName = "staff";
    private readonly IMongoCollection<Academic> _staff;

    public StaffRepository(IMongoDatabase mongoDatabase)
    {
        _staff = mongoDatabase.GetCollection<Academic>(_collectionName);
    }
    
    private static ProjectionDefinition<Academic> GetLanguageProjection(Language language)
    {
        return language switch
        {
            Language.Pl => Builders<Academic>.Projection.Exclude("en"),
            Language.En => Builders<Academic>.Projection.Exclude("pl"),
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }

    public async Task<(IEnumerable<Academic> Staff, Pagination Pagination)> GetStaff(Language language,
        Pagination pagination, 
        string? name,
        CancellationToken cancellationToken)
    {
        var filterBuilder = Builders<Academic>.Filter;
        if (string.IsNullOrEmpty(name)) name = "";
        var filter = !string.IsNullOrEmpty(name) 
            ? filterBuilder.Regex(a => a.Name, new BsonRegularExpression(name, "i")) 
            : filterBuilder.Empty;
        
        var projection = GetLanguageProjection(language);
        projection = projection.Exclude(a => a.Email);
        
        var staff = await _staff.Find(filter)
            .Project<Academic>(projection)
            .Skip((pagination.Page - 1) * pagination.ItemsPerPage)
            .Limit(pagination.ItemsPerPage)
            .ToListAsync(cancellationToken);
        
        var totalStaffRecords = await _staff.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        pagination.TotalPages = Pagination.CalculateTotalPages((int)totalStaffRecords, pagination.ItemsPerPage);
        pagination.HasNextPage = Pagination.CalculateHasNextPage(pagination.Page, pagination.TotalPages);
        return (staff, pagination);
    }
    
    public async Task<Academic?> GetAcademic(string academicId, Language language, CancellationToken cancellationToken)
    {
        var projection = GetLanguageProjection(language);
        
        return await _staff.Find(academic => academic._id == academicId)
                .Project<Academic>(projection)
                .FirstOrDefaultAsync(cancellationToken);
    }
}