using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Events;
using Kiosk.Repositories.Interfaces;
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
    private readonly IEventsRepository _eventsRepository;

    public EventsController(ILogger logger, IEventsService eventsService, IEventsRepository eventsRepository)
    {
        _logger = logger;
        _eventsService = eventsService;
        _eventsRepository = eventsRepository;
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
        [FromBody]List<EventRequest> eventRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            await _eventsService.CreateEvent(eventRequest, cancellationToken);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(string id, [FromBody] EventRequest eventRequest, CancellationToken cancellationToken)
    {
        try
        {
            var updatedEvent = await _eventsService.UpdateEvent(id, eventRequest, cancellationToken);
            return updatedEvent is null ? NotFound() : Ok(updatedEvent);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while updating event. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(string id, CancellationToken cancellationToken)
    {
        try
        {
            var deletedEvent = await _eventsRepository.DeleteEvent(id, cancellationToken);
            return deletedEvent is null ? NotFound() : Ok(deletedEvent);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while deleting event. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }

}