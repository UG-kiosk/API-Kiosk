using Kiosk.Abstractions.Enums;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class TranslatorService : ITranslatorService
{
    private readonly HttpClient _httpClient;

    public TranslatorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

      public async Task<IEnumerable<T>> Translate<T>(IEnumerable<T> text, Language sourceLanguage, IEnumerable<Language> targetLanguages, CancellationToken cancellationToken)
    {
        var azureTranslatorRequest = CreateAzureTranslatorRequest(text);
        var requestUrl = BuildRequestUrl();
        var response = await SendTranslationRequest(requestUrl, azureTranslatorRequest, sourceLanguage, targetLanguages, cancellationToken);
        return await GetTranslatedResults<T>(response);
    }

    private List<Dictionary<string, string>> CreateAzureTranslatorRequest<T>(IEnumerable<T> text)
    {
        return text.Select(item => new Dictionary<string, string>
            {
                { "text", item.ToString() }
            })
            .ToList();
    }

    private string BuildRequestUrl()
    {
        string apiUrl = Environment.GetEnvironmentVariable("SPRING_API_URL");
        string endpoint = "/api/translations";
        return apiUrl.TrimEnd('/') + endpoint;
    }

    private async Task<HttpResponseMessage> SendTranslationRequest(string requestUrl, List<Dictionary<string, string>> azureTranslatorRequest, Language sourceLanguage, IEnumerable<Language> targetLanguages, CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "from", sourceLanguage.ToString() },
            { "targetLanguages", string.Join(",", targetLanguages) }
        };
        var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync(cancellationToken).Result;

        requestUrl += "?" + queryString;

        return await _httpClient.PostAsJsonAsync(requestUrl, azureTranslatorRequest, cancellationToken);
    }

    private async Task<IEnumerable<T>> GetTranslatedResults<T>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<T>>();
    }
}