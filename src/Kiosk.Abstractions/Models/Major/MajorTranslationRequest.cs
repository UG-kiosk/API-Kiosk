using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.Major;

public class MajorTranslationRequest
{
    public required IEnumerable<MajorDetails> MajorsTranslateData { get; set; }
    
    public required Language SourceLanguage { get; set; }
    
    public required IEnumerable<Language> TargetLanguages { get; set; }
}