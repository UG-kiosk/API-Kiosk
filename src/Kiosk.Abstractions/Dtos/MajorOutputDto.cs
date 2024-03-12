using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Dtos;

public class MajorOutputDto
{
    public string? _id { get; set; }
           
    public required string? Url { get; set; }
    
    public required string Name { get; set; }
    
    public required string? Content { get; set; }
    
    public required Degree Degree { get; set; }

    public required Language Language { get; set; }
}