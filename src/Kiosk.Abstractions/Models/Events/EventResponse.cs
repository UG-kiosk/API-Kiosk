using Kiosk.Abstractions.Enums;
using System.Text.Json.Serialization;

namespace Kiosk.Abstractions.Models.Events;

public class EventResponse
{
    public string? _id { get; set; }
    
    public required string Url { get; set; }
    
    public required DateTime Date { get; set; }
    
    public required string Name { get; set; }
    
    public required string Content { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Language Language { get; set; }
}