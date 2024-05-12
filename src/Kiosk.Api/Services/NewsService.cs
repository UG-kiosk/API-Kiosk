using System.Collections.Immutable;
using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.News;
using Kiosk.Abstractions.Models.Translation;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class NewsService : INewsService
{
    private readonly INewsRepository _newsRepository;
    private readonly IMapper _mapper;
    private readonly ITranslatorService _translatorService;
    
    public NewsService(INewsRepository newsRepository, IMapper mapper, ITranslatorService translatorService)
    {
        _newsRepository = newsRepository;
        _mapper = mapper;
        _translatorService = translatorService;
    }

    private NewsResponse MapTranslatedNews(News news, Language language)
    {
        var mappedNews = _mapper.Map<NewsResponse>(news);
        mappedNews.Language = language;

        switch (language)
        {
            case Language.Pl:
                mappedNews.Title = news.Pl.Title;
                mappedNews.ShortBody = news.Pl.ShortBody;
                mappedNews.Body = news.Pl.Body;
                break;
            case Language.En:
                mappedNews.Title = news.En.Title;
                mappedNews.ShortBody = news.En.ShortBody;
                mappedNews.Body = news.En.Body;
                break;
        }

        return mappedNews;
    }

    public async Task<NewsResponse?> GetTranslatedNews(string newsId, Language language, CancellationToken cancellationToken)
    {
        var news = await _newsRepository.GetNews(newsId, cancellationToken);
        return news != null ? MapTranslatedNews(news, language) : null;
    }

    public async Task<(IEnumerable<NewsResponse>?, Pagination Pagination)> GetTranslatedListOfNews(Source? source, Language language, PaginationRequest paginationRequest, CancellationToken cancellationToken)
    {
        var pagination = new Pagination
        {
            Page = paginationRequest.Page,
            ItemsPerPage = paginationRequest.ItemsPerPage
        };
        
        var (newsList, updatedPagination) = await _newsRepository.GetManyNews(source, pagination, cancellationToken);

        return (newsList.Select(news => MapTranslatedNews(news, language)), updatedPagination);
    }

    public async Task CreateNews(IEnumerable<CreateNewsRequest> createNewsRequests, CancellationToken cancellationToken)
    {
        var mappedNews = await TranslateNews(createNewsRequests, cancellationToken);
        await _newsRepository.CreateNews(mappedNews, cancellationToken);
    }

    private async Task<IEnumerable<News>> TranslateNews(
        IEnumerable<CreateNewsRequest> createNewsRequests,
        CancellationToken cancellationToken)
    {
        var groupedByLanguage = createNewsRequests.GroupBy(request => request.SourceLanguage);
        ImmutableList<Language> supportedLanguages = new List<Language> { Language.En, Language.Pl }.ToImmutableList();

        var translationTasks = groupedByLanguage.Select(async newsLanguageGroup =>
        {
            var translationContent = newsLanguageGroup.Select(newsGroup => new TranslationRequest<NewsDetails>
            {
                UniqueKey = Guid.NewGuid().ToString(),
                TranslationPayload = newsGroup.NewsDetails
            }).ToList();

            var translationTask = await _translatorService.Translate(
                translationContent,
                newsLanguageGroup.Key,
                supportedLanguages.Where(language => language != newsLanguageGroup.Key),
                cancellationToken);

            return translationTask.Select(translatedNews =>
            {
                var nativeLanguageNews = translationContent.FirstOrDefault(
                    m => m.UniqueKey == translatedNews.UniqueKey);
                var createNewsDto = createNewsRequests.FirstOrDefault(
                    m => m.NewsDetails.Title == nativeLanguageNews!.TranslationPayload.Title);

                return new News
                {
                    Pl = createNewsDto!.SourceLanguage == Language.Pl
                        ? createNewsDto.NewsDetails
                        : translatedNews.Translations[Language.Pl],
                    En = createNewsDto.SourceLanguage == Language.En
                        ? createNewsDto.NewsDetails
                        : translatedNews.Translations[Language.En],
                    LeadingPhoto = createNewsDto.LeadingPhoto,
                    Photos = createNewsDto.Photos,
                    Link = createNewsDto.Link,
                    Datetime = createNewsDto.Datetime,
                    Source = createNewsDto.Source,
                    Category = createNewsDto.Category
                };
            });
        });
        var translationResults = await Task.WhenAll(translationTasks);
        return translationResults.SelectMany(x => x);
    }
}