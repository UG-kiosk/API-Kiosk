using Kiosk.Abstractions.Enums;
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

    public async Task<IEnumerable<Academic>> GetStaff(ProjectionDefinition<Academic> projection, 
        CancellationToken cancellationToken)
    {
        return await _staff.Find(new BsonDocument()).Project<Academic>(projection)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Academic?> GetAcademic(string academicId, ProjectionDefinition<Academic> projection, 
        CancellationToken cancellationToken)
    {
        return await _staff.Find(academic => academic._id == academicId)
                .Project<Academic>(projection)
                .FirstOrDefaultAsync(cancellationToken);
    }
}