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
    
    public async Task CreateStaff(IEnumerable<Academic> staff, CancellationToken cancellationToken)
    {
        await _staff.InsertManyAsync(staff, cancellationToken: cancellationToken);
    }

    public async Task CreateOrReplaceStaff(IEnumerable<Academic> staff, CancellationToken cancellationToken)
    {
        var operations = new List<WriteModel<Academic>>();

        foreach (var academic in staff)
        {
            if (academic._id == null)
            {
                academic._id = ObjectId.GenerateNewId().ToString(); // Generate a new unique _id if it's null
            }

            var filter = Builders<Academic>.Filter.Eq(a => a.Name, academic.Name);
            var update = Builders<Academic>.Update
                .SetOnInsert(a => a._id, academic._id) // Set _id only on insert
                .Set(a => a.Name, academic.Name)
                .Set(a => a.Email, academic.Email)
                .Set(a => a.Link, academic.Link)
                .Set(a => a.Pl, academic.Pl)
                .Set(a => a.En, academic.En);

            var upsertOneOperation = new UpdateOneModel<Academic>(filter, update) { IsUpsert = true };
            operations.Add(upsertOneOperation);
        }

        await _staff.BulkWriteAsync(operations, cancellationToken: cancellationToken);
    }
    
    public async Task<Academic?> UpdateStaffMember(string academicId, Academic academic, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<Academic>.Update
            .Set(a => a.Name, academic.Name)
            .Set(a => a.Email, academic.Email)
            .Set(a => a.Link, academic.Link)
            .Set(a => a.Pl, academic.Pl)
            .Set(a => a.En, academic.En);
        
        await _staff.UpdateOneAsync(a => a._id == academicId, updateDefinition, cancellationToken: cancellationToken);
        var updatedDocument = await _staff.Find(a => a._id == academicId).FirstOrDefaultAsync(cancellationToken);
        
        return updatedDocument;
    }
    
    public async Task<Academic?> DeleteStaffMember(string academicId, CancellationToken cancellationToken)
    {
        return await _staff.FindOneAndDeleteAsync(a => a._id == academicId, cancellationToken: cancellationToken);
    }
}