using System.ComponentModel.DataAnnotations;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.News;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace KioskAPI.Controllers;

[ApiController]
[Route("news")]
public class NewsController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly INewsService _newsService;
    private readonly INewsRepository _newsRepository;
    public NewsController(ILogger logger, INewsService newsService, INewsRepository newsRepository)
    {
        _logger = logger;
        _newsService = newsService;
        _newsRepository = newsRepository;
    }

    [HttpGet("{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(NewsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetNews(
        string id,
        CancellationToken cancellationToken,
        [FromQuery, Required] Language language)
    {
        try
        {
            var news = await _newsService.GetTranslatedNews(id, language, cancellationToken);
            return Ok(news);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting news. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }

    [HttpGet]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<NewsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetManyNews(
        CancellationToken cancellationToken,
        [FromQuery, Required] Language language,
        [FromQuery] PaginationRequest paginationRequest,
        [FromQuery] Source? source=null,
        [FromQuery] string? search=""
        )
    {
        try
        {
            var (content, pagination) = await _newsService
                .GetTranslatedListOfNews(source, search, language, paginationRequest, cancellationToken);
            return content is null ? NoContent() : Ok(new { content, pagination});
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting news. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateNews(
        [FromBody] List<CreateNewsRequest> createNewsRequests,
        CancellationToken cancellationToken)
    {
        try
        {
            await _newsService.CreateNews(createNewsRequests, cancellationToken);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while creating news. {ExceptionMessage}",
                exception.Message);
            
            return Problem();
        }
    }
    
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(News), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateNews(string id, [FromBody] CreateNewsRequest news, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _newsService.UpdateNews(id, news, cancellationToken);

            return result is null ? NotFound() : Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while updating news. {ExceptionMessage}",
                ex.Message);

            return Problem();
        }
    }

    [HttpDelete("{newsId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(News), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteNews(string newsId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _newsRepository.DeleteNews(newsId, cancellationToken);

            return result is null ? NotFound() : Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while deleting news. {ExceptionMessage}",
                ex.Message);

            return Problem();
        }
    }
}