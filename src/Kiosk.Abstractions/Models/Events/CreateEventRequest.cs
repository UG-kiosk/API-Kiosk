using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.Events;

public class CreateEventRequest
{
    public required EventDetails EventDetails { get; set; }
    public required string? Url { get; set; }
    public required DateTime Date { get; set; }
    public required Language Language { get; set; }
}