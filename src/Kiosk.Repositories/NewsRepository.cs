using System.Linq.Expressions;
using Kiosk.Abstractions.Enums.News;
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

    public async Task<IEnumerable<News>?> GetManyNews(Source? source, CancellationToken cancellationToken)
    {
        Expression<Func<News, bool>> filter = news => source == null || news.Source.ToString() == source.ToString();

        return (await _news.FindAsync(filter, cancellationToken: cancellationToken)).ToEnumerable();
    }
}