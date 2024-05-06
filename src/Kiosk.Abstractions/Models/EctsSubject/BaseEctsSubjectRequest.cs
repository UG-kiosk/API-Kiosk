using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models;

public class BaseEctsSubjectRequest
{
    public string? Speciality { get; set; }
    
    public required Degree Degree { get; set; }

    public required string? Major { get; set; }
    
    public required Language Language { get; set; }
}