using System.Text.Json.Serialization;
using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.Major;

public class MajorResponse
{
    public string? _id { get; set; }
           
    public required string? Url { get; set; }
    
    public required string Name { get; set; }
    
    public required string? Content { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Degree Degree { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Language Language { get; set; }
}