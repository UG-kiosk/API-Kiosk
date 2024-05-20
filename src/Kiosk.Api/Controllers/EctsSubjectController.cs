using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models;
using Kiosk.Abstractions.Models.Major;
using Kiosk.Abstractions.Models.Pagination;
using Kiosk.Abstractions.Models.Translation;
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
    private readonly ITranslatorService _translatorService;
    private readonly ILogger _logger;

    public EctsSubjectController(IEctsSubjectRepository ectsSubjectRepository, IEctsSubjectService ectsSubjectService,
        ILogger logger, ITranslatorService translatorService)
    {
        _ectsSubjectRepository = ectsSubjectRepository;
        _ectsSubjectService = ectsSubjectService;
        _logger = logger;
        _translatorService = translatorService;
    }

    /// <summary>Getting all ects subjects</summary>
    /// <param name="paginationRequest">All pagination info</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">All Ects subjects successfully retrieved</response>
    /// <response code="500">Internal Server Error</response>
    /// <returns>The result of the request, which should contain the list of all Ects Subjects</returns>
    [HttpGet]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EctsSubjectDocument), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEctsSubjects([FromQuery] PaginationRequest paginationRequest,CancellationToken cancellationToken)
    {
        try
        {
            var (ectsSubjects, pagination) = await _ectsSubjectService.GetEcts(paginationRequest, cancellationToken);

            return Ok(new { ectsSubjects, pagination });
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
    /// <param name="ectsSubjectDocument">New ects subject</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Ects subject successfully created</response>
    /// <response code="409">Ects subject wasn't created due to a conflict of subject name, degree or major</response>
    /// <response code="500">Internal Server Error</response>
    /// <returns>The confirmation that the request was processed</returns>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EctsSubjectDocument), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddEctsSubject(EctsSubjectCreateRequest ectsSubjectDocument, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ectsSubjectService.AddEctsSubject(ectsSubjectDocument, cancellationToken);

            return result
                ? Ok("Created successfully")
                : Conflict("Ects Subject with the same subject name, degree or major already exist");
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while getting ectsSubjects. {ExceptionMessage}",
                ex.Message);

            return Problem();
        }
    }
    
    /// <summary>Deleting the ects subject</summary>
    /// <param name="ectsSubjectId">id of subject to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Ects subject successfully deleted</response>
    /// <response code="500">Internal Server Error</response>
    /// <response code="404">Not Found</response>
    /// <returns>The confirmation that the request was processed</returns>
    [HttpDelete("{ectsSubjectId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EctsSubjectDocument), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    
    
    /// <summary>Updating the ects subject</summary>
    /// <param name="ectsSubjectDocument">updated and existing ectsSubjectDocument</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Ects subject successfully updated</response>
    /// <response code="500">Internal Server Error</response>
    /// <response code="404">Not Found</response>
    /// <returns>The confirmation that the request was processed</returns>
    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EctsSubjectDocument), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateEctsSubject(EctsSubjectDocument ectsSubjectDocument, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ectsSubjectRepository.UpdateEctsSubject(ectsSubjectDocument, cancellationToken);

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
    
    /// <summary>Getting lists of majors by degree</summary>
    /// <param name="degree">Degree of majors</param>
    /// <response code="200">The majors have been found successfully</response>
    /// <response code="500">Internal Server Error</response>
    /// <response code="404">Not Found</response>
    /// <returns>The confirmation that the request was processed</returns>
    [HttpGet("{degree}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMajors([FromRoute, Required] Degree degree,[FromQuery, Optional] string? major, [FromQuery] Language language, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ectsSubjectService.GetMajorsOrSpecialities(degree, language, major, cancellationToken);

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
    
    /// <summary>Getting list of ectsSubjects by major</summary>
    /// <param name="ectsSubjectRequest">EctsSubjectRequest</param>
    /// <response code="200">The ectsSubjects have been found successfully</response>
    /// <response code="500">Internal Server Error</response>
    /// <response code="404">Not Found</response>
    /// <returns>The confirmation that the request was processed</returns>
    [HttpGet("major")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSubjectsByMajor([FromQuery] EctsSubjectRequest ectsSubjectRequest, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ectsSubjectService.GetSubjectsByMajor(ectsSubjectRequest, cancellationToken);
    
            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while getting ects subjects. {ExceptionMessage}",
                ex.Message);
    
            return Problem();
        }
    }
    
    /// <summary>Getting lists of years by major or speciality</summary>
    /// <param name="ectsSubjectRequest">EctsSubjectRequest</param>
    /// <response code="200">The years have been found successfully</response>
    /// <response code="500">Internal Server Error</response>
    /// <response code="404">Not Found</response>
    /// <returns>The confirmation that the request was processed</returns>
    [HttpGet("years")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetYears([FromQuery, Required] BaseEctsSubjectRequest baseEctsSubjectRequest, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ectsSubjectService.GetYears(baseEctsSubjectRequest, cancellationToken);
    
            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                "Something went wrong while getting years. {ExceptionMessage}",
                ex.Message);
    
            return Problem();
        }
    }
}
