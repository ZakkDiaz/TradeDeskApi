using System.Security.Claims;

namespace TradeDeskTop.Services
{
    public interface IApiKeyService
    {
        Task<ClaimsPrincipal> ValidateApiKey(string apiKey);
    }
}