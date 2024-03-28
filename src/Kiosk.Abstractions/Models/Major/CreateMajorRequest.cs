using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.Major;

public class CreateMajorRequest
{
    public required MajorDetails MajorDetails { get; set; }
    
    public required string? Url { get; set; }
    
    public required Degree Degree { get; set; }
    
    public required Language SourceLanguage { get; set; }
}