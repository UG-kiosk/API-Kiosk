using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using KioskAPI.Services.Interfaces;

namespace KioskAPI.Filters
{
    public class ValidateTokenFilter : IAsyncActionFilter
    {
        private readonly IAuthService _authService;

        public ValidateTokenFilter(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var authResponse = await _authService.ValidateRequest(context.HttpContext, context.HttpContext.RequestAborted);
            var jwtValue = _authService.ExtractToken(authResponse);
            if (jwtValue == null || !authResponse.IsSuccessStatusCode)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.Response.Cookies.Append("jwt", jwtValue, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromHours(1)
            });
            
            await next();
        }
    }
}