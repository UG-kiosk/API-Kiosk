using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Translation;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class TranslatorService : ITranslatorService
{
    private readonly HttpClient _httpClient;

    public TranslatorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

      public async Task<IEnumerable<TranslationResponse<T>>> Translate<T>(IEnumerable<TranslationRequest<T>> translationContent, Language sourceLanguage, IEnumerable<Language> targetLanguages, CancellationToken cancellationToken)
    {
        var requestUrl = BuildRequestUrl();
        var response = await SendTranslationRequest<T>(requestUrl, translationContent, sourceLanguage, targetLanguages, cancellationToken);
        return await GetTranslatedResults<T>(response, targetLanguages);
    }

    private string BuildRequestUrl()
    {
        string apiUrl = Environment.GetEnvironmentVariable("SPRING_API_URL");
        string endpoint = "/translations-kiosk-api/api/translations";
        return apiUrl.TrimEnd('/') + endpoint;
    }

    private async Task<HttpResponseMessage> SendTranslationRequest<T>(string requestUrl, IEnumerable<TranslationRequest<T>> translationContent, Language sourceLanguage, IEnumerable<Language> targetLanguages, CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "from", sourceLanguage.ToString() },
            { "targetLanguages", string.Join(",", targetLanguages) }
        };
        var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync(cancellationToken).Result;

        requestUrl += "?" + queryString;

        return await _httpClient.PostAsJsonAsync(requestUrl, translationContent, cancellationToken);
    }

    private async Task<IEnumerable<TranslationResponse<T>>> GetTranslatedResults<T>(HttpResponseMessage response, IEnumerable<Language> targetLanguages)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<TranslationResponse<T>>>();
    }
}