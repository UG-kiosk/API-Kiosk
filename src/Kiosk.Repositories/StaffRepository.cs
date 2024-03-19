using Kiosk.Abstractions.Models;
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

    public async Task<(IEnumerable<Academic> Staff, Pagination Pagination)> GetStaff(
        ProjectionDefinition<Academic> projection, Pagination pagination, string? name, CancellationToken cancellationToken)
    {
        var filterBuilder = Builders<Academic>.Filter;
        if (string.IsNullOrEmpty(name)) name = "";
        var filter = !string.IsNullOrEmpty(name) 
            ? filterBuilder.Regex(a => a.Name, new BsonRegularExpression(name, "i")) 
            : filterBuilder.Empty;
        
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
    
    public async Task<Academic?> GetAcademic(string academicId, ProjectionDefinition<Academic> projection, 
        CancellationToken cancellationToken)
    {
        return await _staff.Find(academic => academic._id == academicId)
                .Project<Academic>(projection)
                .FirstOrDefaultAsync(cancellationToken);
    }
}