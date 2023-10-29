using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TradeDeskApi.Common.Authorization;

namespace TradeDeskTop.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly Dictionary<string, string> _apiKeys;

        public ApiKeyService(IOptions<ApiKeyConfig> apiKeyConfig)
        {
            _apiKeys = apiKeyConfig.Value.Keys;
        }

        public bool ValidateApiKey(string apiKey, out ClaimsPrincipal claimsPrincipal)
        {
            claimsPrincipal = null;
            string apiIdentityName = "UNKNOWN";
            if (!_apiKeys.TryGetValue(apiKey, out apiIdentityName))
                return false;
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, apiIdentityName) }, "User"));
            return true;
        }
    }
}
