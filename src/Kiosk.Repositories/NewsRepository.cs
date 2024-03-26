using System.Linq.Expressions;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.News;
using Kiosk.Repositories.Interfaces;
using MongoDB.Driver;

namespace Kiosk.Repositories;

public class NewsRepository : INewsRepository
{
    private readonly string _collectionName = "news";
    private readonly IMongoCollection<News> _news;

    public NewsRepository(IMongoDatabase mongoDatabase)
    {
        _news = mongoDatabase.GetCollection<News>(_collectionName);
    }

    public async Task<News?> GetNews(string id, CancellationToken cancellationToken)
        => await _news.Find(news => news._id == id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<(IEnumerable<News>?, Pagination Pagination)> GetManyNews(Source? source, Pagination pagination, CancellationToken cancellationToken)
    {
        Expression<Func<News, bool>> filter = news => source == null || news.Source.ToString() == source.ToString();

        var news = await _news.Find(filter).Skip((pagination.Page - 1) * pagination.ItemsPerPage)
            .SortByDescending(news => news.Datetime)
            .Limit(pagination.ItemsPerPage)
            .ToListAsync(cancellationToken);
        
        var totalStaffRecords = await _news.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        pagination.TotalPages = Pagination.CalculateTotalPages((int)totalStaffRecords, pagination.ItemsPerPage);
        pagination.HasNextPage = Pagination.CalculateHasNextPage(pagination.Page, pagination.TotalPages);
        return (news, pagination);
    }
}