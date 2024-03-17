using System.ComponentModel.DataAnnotations;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.LessonPlan;
using KioskAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
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

    [HttpGet("{name}/{year}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<LessonPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllLessonsForMajorYear(
        [FromRoute] BaseLessonPlanRequest baseLessonPlanRequest,
        [FromQuery, Required] Language language,
        CancellationToken cancellationToken)
    {
        try
        {
            var lessonsList = await _lessonPlanService
                .GetAllLessonsForMajorYear(baseLessonPlanRequest, language, cancellationToken);
            
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
    
    [HttpGet("{name}/{year}/lectures")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<LessonPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllLecturesForMajorYear(
        [FromRoute] BaseLessonPlanRequest baseLessonPlanRequest,
        [FromQuery, Required] Language language,
        CancellationToken cancellationToken)
    {
        try
        {
            var lessonsList = await _lessonPlanService
                .GetAllLecturesForMajorYear(baseLessonPlanRequest, language, cancellationToken);

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
    
    [HttpGet("{name}/{year}/{group}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<LessonPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllLessonsForMajorYearGroup(
        [FromRoute] LessonPlanGroupRequest lessonPlanGroupRequest,
        [FromQuery, Required] Language language,
        CancellationToken cancellationToken)
    {
        try
        {
            var lessonsList = await _lessonPlanService
                .GetAllLessonsForMajorYearGroup(lessonPlanGroupRequest, language, cancellationToken);

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
}