using System.Net.Http.Headers;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    
    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<HttpResponseMessage> ValidateRequest(HttpContext httpContext, CancellationToken cancellationToken)
    {
        _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("AUTH_API_URL")!);
        
        var token = httpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var authResponse = await _httpClient.GetAsync("/api/auth", cancellationToken);
        
        return authResponse;
    }

    public string? ExtractToken(HttpResponseMessage authResponse)
    {
        var cookies = authResponse.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
        var jwtCookie = cookies?.FirstOrDefault(c => c.StartsWith("jwt="));
        return jwtCookie?.Split(';').First().Split('=').Last();
    }
}