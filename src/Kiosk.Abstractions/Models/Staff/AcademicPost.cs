namespace Kiosk.Abstractions.Models.Staff;

public class AcademicPost
{
    public required string Position { get; set; } 
    
    public required List<string> Faculty { get; set; }
}