using System.ComponentModel.DataAnnotations;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.LessonPlan;
using KioskAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace KioskAPI.Controllers;

[ApiController]
[Route("lessonsPlans")]
public class LessonPlanController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly ILessonPlanService _lessonPlanService;

    public LessonPlanController(ILogger logger, ILessonPlanService lessonPlanService)
    {
        _logger = logger;
        _lessonPlanService = lessonPlanService;
    }
    
    [HttpGet("lectures")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<GetLessonPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllLecturesForMajorYear(
        [FromQuery] GetLessonsPlanRequestLectures getLessonsPlanRequestLectures,
        [FromQuery, Required] Language language,
        CancellationToken cancellationToken)
    {
        try
        {
            var lessonsList = await _lessonPlanService
                .GetAllLecturesForMajorYear(getLessonsPlanRequestLectures, language, cancellationToken);

            return lessonsList is null ? NotFound() : Ok(lessonsList);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting lessons. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }
    
    [HttpGet]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<GetLessonPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllLessonsForMajorYearGroup(
        [FromQuery] GetLessonPlanRequest getLessonPlanRequest,
        [FromQuery, Required] Language language,
        CancellationToken cancellationToken)
    {
        try
        {
            var lessonsList = await _lessonPlanService
                .GetAllLessonsForMajorYearGroup(getLessonPlanRequest, language, cancellationToken);

            return lessonsList is null ? NotFound() : Ok(lessonsList);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting lessons. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }
    
     
    [HttpGet("majors")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMajors(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _lessonPlanService.GetMajors(cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while getting majors. {ExceptionMessage}",
                ex.Message);

            return Problem();
        }
    }
    
    [HttpGet("{major}/years")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMajorYears([FromRoute, Required] string major, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _lessonPlanService.GetMajorYears(major, cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while getting majors. {ExceptionMessage}",
                ex.Message);

            return Problem();
        }
    }
    
    [HttpGet("{major}/{year}/groups")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMajorGroups([FromRoute, Required] string major, [FromRoute, Required] int year, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _lessonPlanService.GetMajorGroups(major, year, cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while getting majors. {ExceptionMessage}",
                ex.Message);

            return Problem();
        }
    }
}