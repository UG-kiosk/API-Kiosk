using Kiosk.Abstractions.Dtos;
using Kiosk.Abstractions.Enums;
using KioskAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace KioskAPI.Controllers;

[ApiController]
[Route("majors")]
public class MajorsController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IMajorsService _majorsService;

    public MajorsController(ILogger logger, IMajorsService majorsService)
    {
        _logger = logger;
        _majorsService = majorsService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMajor(
        string id,
        CancellationToken cancellationToken,
        [FromQuery(Name = "language")] Language language)

    {
        try
        {
            var major = await _majorsService.GetTranslatedMajor(id, language, cancellationToken);
            
            return Ok(major);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting major. {ExceptionMessage}",
                exception.Message);
            
            return Problem();
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMajors(
        [FromQuery] FindMajorsQueryDto findMajorsQueryDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var majorsList = await _majorsService
                .GetTranslatedMajors(findMajorsQueryDto, cancellationToken);
            
            return Ok(majorsList);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting majors. {ExceptionMessage}",
                exception.Message);
            
            return Problem();
        }
    }
}