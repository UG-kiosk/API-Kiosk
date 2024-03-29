using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.News;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class NewsService : INewsService
{
    private readonly INewsRepository _newsRepository;
    private readonly IMapper _mapper;
    
    public NewsService(INewsRepository newsRepository, IMapper mapper)
    {
        _newsRepository = newsRepository;
        _mapper = mapper;
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

    public async Task<IEnumerable<NewsResponse>?> GetTranslatedListOfNews(Source? source, Language language, CancellationToken cancellationToken)
    {
            var newsList = await _newsRepository.GetManyNews(source, cancellationToken);

            return newsList?.Select(news => MapTranslatedNews(news, language));
    }
}