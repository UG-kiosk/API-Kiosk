using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Enums.News;

namespace Kiosk.Abstractions.Models.News;

public class CreateNewsRequest
{
    public required string LeadingPhoto { get; set; }
    
    public List<string> Photos { get; set; }
    
    public required string Link { get; set; }
    
    public required DateTime Datetime { get; set; }
    
    public required NewsDetails NewsDetails { get; set; }
    
    public required Source Source { get; set; }

    public required Category Category { get; set; }

    public Language SourceLanguage { get; set; } = Language.Pl;

}