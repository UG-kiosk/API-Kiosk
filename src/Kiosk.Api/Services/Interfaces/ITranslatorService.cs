using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Translation;

namespace KioskAPI.Services.Interfaces;

public interface ITranslatorService
{
    public Task<IEnumerable<TranslationResponse<T>>> Translate<T>(IEnumerable<TranslationRequest<T>> translationContent, Language sourceLanguage, IEnumerable<Language> targetLanguages, CancellationToken cancellationToken);
}