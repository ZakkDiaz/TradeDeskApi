using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TradeDeskApi.Common.Authorization;
using TradeDeskData;

namespace TradeDeskTop.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IFinancialRepository _financialRepo;

        public ApiKeyService(IFinancialRepository financialRepo)
        {
            _financialRepo = financialRepo;
        }

        public async Task<ClaimsPrincipal> ValidateApiKey(string apiKey)
        {
            var userProfile = await _financialRepo.GetUserProfileByKeyAsync(apiKey);
            if (userProfile == null)
                return null;
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, userProfile.Name), new Claim("UserId", userProfile.Id.ToString()) }, "User"));
        }
    }
}
