using System.ComponentModel.DataAnnotations;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Staff;
using Kiosk.Repositories.Interfaces;
using KioskAPI.Filters;
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
    private readonly IStaffRepository _staffRepository;

    public StaffController(ILogger logger, IStaffService staffService, IStaffRepository staffRepository)
    {
        _logger = logger;
        _staffService = staffService;
        _staffRepository = staffRepository;
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
    [ServiceFilter(typeof(ValidateTokenFilter))]
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
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStaffMember(string id, [FromBody] AcademicRequest staff, CancellationToken cancellationToken)
    {
        try
        {
            var updatedStaffMember = await _staffService.UpdateStaffMember(id, staff, cancellationToken);
            return updatedStaffMember is null ? NotFound() : Ok(updatedStaffMember);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while updating staff member. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaffMember(string id, CancellationToken cancellationToken)
    {
        try
        {
            var deletedStaffMember = await _staffRepository.DeleteStaffMember(id, cancellationToken);
            return deletedStaffMember is null ? NotFound() : Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                "Something went wrong while deleting staff member. {ExceptionMessage}",
                exception.Message);

            return Problem();
        }
    }
}