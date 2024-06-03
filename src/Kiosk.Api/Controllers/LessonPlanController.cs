using System.ComponentModel.DataAnnotations;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.LessonPlan;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Repositories.Interfaces;
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
    private readonly ILessonPlanRepository _lessonPlanRepository;
    public LessonPlanController(ILogger logger, ILessonPlanService lessonPlanService, ILessonPlanRepository lessonPlanRepository)
    {
        _logger = logger;
        _lessonPlanService = lessonPlanService;
        _lessonPlanRepository = lessonPlanRepository;
    }
    
    [HttpGet("all")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<GetLessonPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllLessons(
        [FromQuery, Required] Language language,
        [FromQuery] PaginationRequest paginationRequest,
        CancellationToken cancellationToken,
        [FromQuery] string? search="",
        [FromQuery] string? day=""
        )
    {
        try
        {
            var (content, pagination)  = await _lessonPlanService
                .GetAllLessons(day, search, language, paginationRequest, cancellationToken);

            return content is null ? NoContent() : Ok(new { content, pagination});
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting lessons. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
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
    
    [HttpPost]
    public async Task<IActionResult> CreateLessons(
        [FromBody] List<CreateLessonPlanRequest> createLessonPlanRequests,
        CancellationToken cancellationToken)
    {
        try
        {
            await _lessonPlanService.CreateLessons(createLessonPlanRequests, cancellationToken);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while creating lessons. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }
    
    [HttpDelete("{lessonsId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LessonPlan), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteLesson(string lessonsId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _lessonPlanRepository.DeleteLesson(lessonsId, cancellationToken);

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