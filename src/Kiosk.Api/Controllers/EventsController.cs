using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Events;
using Kiosk.Abstractions.Models.Pagination;
using KioskAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;


namespace KioskAPI.Controllers;

[ApiController]
[Route("events")]
public class EventsController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IEventsService _eventsService;

    public EventsController(ILogger logger, IEventsService eventsService)
    {
        _logger = logger;
        _eventsService = eventsService;
    }
    
    [HttpGet("{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> GetEvent(
        string id,
        Language language,
        CancellationToken cancellationToken
        )
    {
        try
        {
            var events = await _eventsService.GetTranslatedEvent(id, language, cancellationToken);
            return Ok(events);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting event. {ExceptionMessage}",
                exception.Message);
            
            return Problem();
        }
    }
    
    [HttpGet]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> GetEvents(
        [FromQuery] int? page,
        [FromQuery] int? itemsPerPage,
        Language language,
        CancellationToken cancellationToken)
    
    {
        try
        {
            var (content, pagination) = await _eventsService
                .GetTranslatedEvents(language, page, itemsPerPage, cancellationToken);

            return content is null ? NoContent() : Ok(new {content, pagination});
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting events. {ExceptionMessage}",
                exception.Message);
            
            return Problem();
        }
    }
    
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> CreateEvent(
        [FromBody]List<CreateEventRequest> createEventRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            await _eventsService.CreateEvent(createEventRequest, cancellationToken);
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while creating event. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }

}