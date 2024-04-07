namespace Kiosk.Abstractions.Models.Staff;

public class AcademicSimplifiedContent
{
    public required List<AcademicSimplifiedPost> Posts { get; set; }
    
    public required string Tutorial { get; set; }
}