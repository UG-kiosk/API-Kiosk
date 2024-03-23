using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.News;

namespace KioskAPI.Services.Interfaces;

public interface INewsService
{
    Task<NewsResponse?> GetTranslatedNews(string newsId, Language language, CancellationToken cancellationToken);

    Task<(IEnumerable<NewsResponse>?, Pagination Pagination)> GetTranslatedListOfNews(Source? source, Language language, int page, int itemsPerPage, CancellationToken cancellationToken);
}