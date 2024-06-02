using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.News;
using Kiosk.Abstractions.Models.Pagination;

namespace KioskAPI.Services.Interfaces;

public interface INewsService
{
    Task<NewsResponse?> GetTranslatedNews(string newsId, Language language, CancellationToken cancellationToken);

    Task<(IEnumerable<NewsResponse>?, Pagination Pagination)> GetTranslatedListOfNews(Source? source, string? search,
        Language language, PaginationRequest paginationRequest, CancellationToken cancellationToken);
    
    Task CreateNews(IEnumerable<CreateNewsRequest> createNewsRequests, CancellationToken cancellationToken);
    Task<NewsResponse?> UpdateNews(string id, CreateNewsRequest news, CancellationToken cancellationToken);
}