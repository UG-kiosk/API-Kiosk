using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.News;

namespace Kiosk.Repositories.Interfaces;

public interface INewsRepository
{
    Task<News?> GetNews(string id, CancellationToken cancellationToken);

    Task<IEnumerable<News>?> GetManyNews(Source? source, CancellationToken cancellationToken);
}