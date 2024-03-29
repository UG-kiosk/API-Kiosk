using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.Translation;

public class TranslationResponse<T>
{
    public String UniqueKey { get; set; }
    public Dictionary<Language, T> Translations { get; set; }
}