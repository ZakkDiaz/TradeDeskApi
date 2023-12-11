using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TradeDeskTop.Services;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private const string AuthCookieName = "AuthCookie";
    private readonly IApiKeyService _apiKeyService;

    public ApiKeyAuthenticationHandler(
        IApiKeyService apiKeyService,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _apiKeyService = apiKeyService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if a valid AuthCookie exists
        if (Context.Request.Cookies.TryGetValue(AuthCookieName, out var authCookie))
        {
            var claim = await _apiKeyService.ValidateApiKey(authCookie);
            if (claim != null)
            {
                var ticket = new AuthenticationTicket(claim, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
        }

        // Fallback to API Key authentication
        if (!Context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
        {
            return AuthenticateResult.Fail("API Key was not provided.");
        }

        var claimFromApiKey = await _apiKeyService.ValidateApiKey(apiKey);
        if (claimFromApiKey == null)
        {
            return AuthenticateResult.Fail("Invalid API Key.");
        }

        // Set secure, HttpOnly cookie for future requests
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Set this to false if you're not using HTTPS
            IsEssential = true
        };
        Context.Response.Cookies.Append(AuthCookieName, apiKey, cookieOptions);

        var ticketFromApiKey = new AuthenticationTicket(claimFromApiKey, Scheme.Name);
        return AuthenticateResult.Success(ticketFromApiKey);
    }
}