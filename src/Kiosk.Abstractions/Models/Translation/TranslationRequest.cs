namespace Kiosk.Abstractions.Models.Translation;

public class TranslationRequest<T>
{
    public string UniqueKey { get; set; }
    public T TranslationPayload { get; set; }
}