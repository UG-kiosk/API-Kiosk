using Kiosk.Abstractions.Enums;

namespace KioskAPI.Services.Interfaces;

public interface ITranslatorService
{
    public Task<IEnumerable<T>> Translate<T>(IEnumerable<T> text, Language sourceLanguage, IEnumerable<Language> targetLanguages, CancellationToken cancellationToken);
}