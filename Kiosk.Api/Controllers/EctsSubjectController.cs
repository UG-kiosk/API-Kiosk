using Kiosk.Abstractions.Models;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace KioskAPI.Controllers;

[ApiController]
[Route("ects-subjects")]
public class EctsSubjectController : ControllerBase
{
    private readonly IEctsSubjectRepository _ectsSubjectRepository;
    private readonly IEctsSubjectService _ectsSubjectService;
    private readonly ILogger _logger;

    public EctsSubjectController(IEctsSubjectRepository ectsSubjectRepository, IEctsSubjectService ectsSubjectService,
        ILogger logger)
    {
        _ectsSubjectRepository = ectsSubjectRepository;
        _ectsSubjectService = ectsSubjectService;
        _logger = logger;
    }

    /// <summary>Getting all ects subjects</summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">All Ects subjects successfully retrieved</response>
    /// <response code="500">Internal Server Error</response>
    /// <returns>The result of the request, which should contain the list of all Ects Subjects</returns>
    [HttpGet]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EctsSubject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEctsSubjects(CancellationToken cancellationToken)
    {
        try
        {
            var ectsSubjects = await _ectsSubjectRepository.GetEctsSubjects(cancellationToken);

            return Ok(ectsSubjects);
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while getting ectsSubjects. {ExceptionMessage}",
                ex.Message);

            return Problem();
        }
    }

    /// <summary>Adding the ects subject</summary>
    /// <param name="ectsSubject">New ects subject</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Ects subject successfully created</response>
    /// <response code="409">Ects subject wasn't created due to a conflict of subject name, degree or major</response>
    /// <response code="500">Internal Server Error</response>
    /// <returns>The confirmation that the request was processed</returns>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EctsSubject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddEctsSubject(EctsSubject ectsSubject, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ectsSubjectService.AddEctsSubject(ectsSubject, cancellationToken);

            return result
                ? Ok("Created successfully")
                : Conflict("Ects Subject with the same subject name, degree or major allready exist");
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while getting ectsSubjects. {ExceptionMessage}",
                ex.Message);

            return Problem();
        }
    }

    [HttpDelete("{ectsSubjectId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EctsSubject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteEctsSubject(string ectsSubjectId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ectsSubjectRepository.DeleteEctsSubject(ectsSubjectId, cancellationToken);

            return result is null ? NotFound() : Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while deleting ectsSubject. {ExceptionMessage}",
                ex.Message);

            return Problem();
        }
    }

    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EctsSubject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateEctsSubject(EctsSubject ectsSubject, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ectsSubjectRepository.UpdateEctsSubject(ectsSubject, cancellationToken);

            return result is null ? NotFound() : Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while updating ectsSubject. {ExceptionMessage}",
                ex.Message);

            return Problem();
        }
    }
}