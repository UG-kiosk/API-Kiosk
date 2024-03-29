using System.ComponentModel.DataAnnotations;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using Kiosk.Abstractions.Models.News;
using Kiosk.Abstractions.Models.Pagination;
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

    public NewsController(ILogger logger, INewsService newsService)
    {
        _logger = logger;
        _newsService = newsService;
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
        [FromQuery] Source? source=null
        )
    {
        try
        {
            var (content, pagination) = await _newsService
                .GetTranslatedListOfNews(source, language, paginationRequest, cancellationToken);
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
}