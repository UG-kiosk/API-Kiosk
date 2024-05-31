using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.News;

namespace Kiosk.Repositories.Interfaces;

public interface INewsRepository
{
    Task<News?> GetNews(string id, CancellationToken cancellationToken);

    Task<(IEnumerable<News>?, Pagination Pagination)> GetManyNews(Source? source, Pagination pagination, CancellationToken cancellationToken);
    
    Task CreateNews(IEnumerable<News> news, CancellationToken cancellationToken);
    Task<News?> DeleteNews(string newsId, CancellationToken cancellationToken);
    Task<News> UpdateNews(string id, News news, CancellationToken cancellationToken);
}