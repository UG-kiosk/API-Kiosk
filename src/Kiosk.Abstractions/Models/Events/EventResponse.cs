using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.Events;

public class EventResponse
{
    public string? _id { get; set; }
    
    public required string Url { get; set; }
    
    public required string Name { get; set; }
    
    public required string Content { get; set; }
    
    public required Language Language { get; set; }
}