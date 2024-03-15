using System.Text.Json.Serialization;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;

namespace Kiosk.Abstractions.Models.News;

public class NewsResponse
{
    public string? _id { get; set; }
    
    public required string LeadingPhoto { get; set; }
    
    public List<string> Photos { get; set; }
    
    public required string Link { get; set; }
    
    public required DateOnly Datetime { get; set; }
    
    public required string Title { get; set; }
    
    public required string ShortBody { get; set; }
    
    public required string Body { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Source Source { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Category Category { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Language Language { get; set; }

}