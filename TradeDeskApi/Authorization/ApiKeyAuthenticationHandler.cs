using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TradeDeskTop.Services;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string ApiKeyHeaderName = "X-Api-Key";
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
        if (!Context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
        {
            return AuthenticateResult.Fail("API Key was not provided.");
        }

        var claim = await _apiKeyService.ValidateApiKey(apiKey);
        if (claim == null)
        {
            return AuthenticateResult.Fail("Invalid API Key.");
        }

        var ticket = new AuthenticationTicket(claim, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}