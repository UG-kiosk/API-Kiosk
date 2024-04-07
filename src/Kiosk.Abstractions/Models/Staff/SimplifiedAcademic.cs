namespace Kiosk.Abstractions.Models.Staff;

public class SimplifiedAcademic
{
    public string? _id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Link { get; set; }
    
    public required string Email { get; set; }
    
    public required AcademicSimplifiedContent Content { get; set; }
    
}