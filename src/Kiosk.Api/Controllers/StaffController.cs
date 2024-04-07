using System.ComponentModel.DataAnnotations;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Staff;
using KioskAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace KioskAPI.Controllers;

[ApiController]
[Route("staff")]
public class StaffController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IStaffService _staffService;

    public StaffController(ILogger logger, IStaffService staffService)
    {
        _logger = logger;
        _staffService = staffService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStaff([FromQuery] [Required] Language language, 
        [FromQuery] int? page, 
        [FromQuery] int? itemsPerPage,
        [FromQuery] string? name,
        CancellationToken cancellationToken)
    { 
        try
        {
            var (response, pagination) = await _staffService.GetStaff(language, page, 
                itemsPerPage, name, cancellationToken);
            
            return Ok(new { response, pagination });
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting staff collection. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStaffMember(string id, [FromQuery] [Required] Language language,
        CancellationToken cancellationToken)
    {
        try
        {
            var staffMember = await _staffService.GetStaffMember(id, language, cancellationToken);
            return staffMember is null ? NotFound() : Ok(staffMember);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while getting staff member. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] IEnumerable<AcademicRequest> staff, CancellationToken cancellationToken)
    {
        try
        {
            await _staffService.CreateStaff(staff, cancellationToken);
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while creating staff. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }
}