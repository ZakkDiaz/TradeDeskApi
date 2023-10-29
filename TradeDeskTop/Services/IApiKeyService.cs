using System.Security.Claims;

namespace TradeDeskTop.Services
{
    public interface IApiKeyService
    {
        bool ValidateApiKey(string apiKey, out ClaimsPrincipal claimsPrincipal);
    }
}