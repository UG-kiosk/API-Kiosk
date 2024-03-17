namespace Kiosk.Abstractions.Models.Staff;

public class AcademicContent
{
    public required List<AcademicPost> Posts { get; set; }
    
    public required string Tutorial { get; set; }
}