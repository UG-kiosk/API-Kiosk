using System.Collections;
using System.Text.RegularExpressions;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Major;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Repositories.Interfaces;
using MongoDB.Bson;
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

    public async Task<IEnumerable<EctsSubjectDocument>?> GetEctsSubjectsByName(string subject,Language language, CancellationToken cancellationToken)
        => (await _ectsSubjects.FindAsync(r => r[language].Subject == subject, cancellationToken: cancellationToken))
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

    public async Task<IEnumerable<string>?> GetMajors(Degree degree, Language language, CancellationToken cancellationToken)=>
        (await _ectsSubjects.DistinctAsync<string>( $"{language}.major", DegreeFilter(degree),cancellationToken: cancellationToken)).ToEnumerable();
    
    public async Task<IEnumerable<string?>?> GetSpecialities(Degree degree, string major, Language language, CancellationToken cancellationToken)
        => (await _ectsSubjects.DistinctAsync<string>($"{language}.subject", DegreeFilter(degree) & MajorOrSpecialityFilter(major, null, language), cancellationToken: cancellationToken)).ToEnumerable();
    
    public async Task<IEnumerable<EctsSubjectDocument>?> GetSubjectsByMajor(EctsSubjectRequest ectsSubject, CancellationToken cancellationToken)
    {
        var yearFilter = Builders<EctsSubjectDocument>.Filter.Where(x => x.RecruitmentYear.Contains(ectsSubject.Year));
        
        var updatedDocument = (await _ectsSubjects
            .FindAsync(MajorOrSpecialityFilter(ectsSubject.Major, ectsSubject.Speciality, ectsSubject.Language) & DegreeFilter(ectsSubject.Degree) & yearFilter, cancellationToken: cancellationToken))
            .ToEnumerable();
    
        return updatedDocument;
    }
    
    

    public async Task<IEnumerable<int>?> GetYears(BaseEctsSubjectRequest baseEctsSubjectRequest, CancellationToken cancellationToken) => 
        (await _ectsSubjects.DistinctAsync(e => e.RecruitmentYear.First(), MajorOrSpecialityFilter(baseEctsSubjectRequest.Major, baseEctsSubjectRequest.Speciality, baseEctsSubjectRequest.Language) & DegreeFilter(baseEctsSubjectRequest.Degree)))
        .ToList();

    public async Task<(IEnumerable<EctsSubjectDocument>?, Pagination pagination)> GetEctsByPagination(PaginationRequest paginationRequest, CancellationToken cancellationToken)
    {
        var filterBuilder = Builders<EctsSubjectDocument>.Filter;
        var filterValue = paginationRequest.filterValue;
        if (string.IsNullOrEmpty(filterValue)) filterValue = "";
        
        var subjectFilter = !string.IsNullOrEmpty(filterValue) 
            ? filterBuilder.Regex("Pl.subject", new BsonRegularExpression(filterValue, "im")) 
            : filterBuilder.Empty;

        var degreeFilter = DegreeFilter(paginationRequest.Degree ?? Degree.Bachelor);

        var sort = SortByValue(paginationRequest.sortDirection ?? Sorting.Asc, paginationRequest.sortBy ?? "subject");

        var staff = await _ectsSubjects.Find(subjectFilter & degreeFilter).Sort(sort)
            .Skip((paginationRequest.Page - 1) * paginationRequest.ItemsPerPage)
            .Limit(paginationRequest.ItemsPerPage)
            .ToListAsync(cancellationToken);
        
        var pagination = new Pagination()
        {
            Page = paginationRequest.Page,
            ItemsPerPage = paginationRequest.ItemsPerPage
        };
        
        var totalStaffRecords = await _ectsSubjects.CountDocumentsAsync(subjectFilter & degreeFilter, cancellationToken: cancellationToken);
        pagination.TotalPages = Pagination.CalculateTotalPages((int)totalStaffRecords, pagination.ItemsPerPage);
        pagination.HasNextPage = Pagination.CalculateHasNextPage(pagination.Page, pagination.TotalPages);
        return (staff, pagination);
        
    }


    private FilterDefinition<EctsSubjectDocument> MajorOrSpecialityFilter(string? major, string? speciality, Language language)
    {
        if (speciality == null)
        {
            return Builders<EctsSubjectDocument>.Filter.Eq($"{language}.major", major);
        }
        else
        {
            return Builders<EctsSubjectDocument>.Filter.Eq($"{language}.speciality", speciality);
        }
    }

    private SortDefinition<EctsSubjectDocument> SortByValue(Sorting sortDirection, string sortingValue)
    {
        if (sortDirection == Sorting.Asc)
        {
            return Builders<EctsSubjectDocument>.Sort.Ascending($"Pl.{sortingValue}");
        }
        else
        {
            return Builders<EctsSubjectDocument>.Sort.Descending($"Pl.{sortingValue}");
        }

    }
    
    
    
    private FilterDefinition<EctsSubjectDocument> DegreeFilter(Degree degree) =>
        Builders<EctsSubjectDocument>.Filter.Eq(r => r.Degree, degree);
}