namespace KioskAPI.Services.Interfaces;

public interface IAuthService
{
    public Task<HttpResponseMessage> ValidateRequest(HttpContext httpContext, CancellationToken cancellationToken);
    public string? ExtractToken(HttpResponseMessage authResponse);
}