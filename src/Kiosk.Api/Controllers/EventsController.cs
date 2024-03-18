using System.ComponentModel.DataAnnotations;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;
using KioskAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;


namespace KioskAPI.Controllers;

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
    public async Task<IActionResult> GetEvent(
        string id,
        CancellationToken cancellationToken,
        [FromQuery(Name = "language"), Required] Language language)
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
    public async Task<IActionResult> GetEvents(
        CancellationToken cancellationToken,
        [FromQuery(Name = "language"), Required] Language language,
        [FromQuery] Source? source=null)
    
    {
        try
        {
            var eventsList = await _eventsService
                .GetTranslatedEvents(source, language, cancellationToken);

            return eventsList is null ? NoContent() : Ok(eventsList);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting events. {ExceptionMessage}",
                exception.Message);
            
            return Problem();
        }
    }
}