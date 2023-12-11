using Microsoft.AspNetCore.Mvc;
using TradeDeskBroker.Market;

namespace TradeDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketController : BaseController
    {
        IMarketService _marketService;
        public MarketController(IMarketService marketService)
        {
            _marketService = marketService;
        }

        [HttpGet]
        [Route("getMarketForSymbol/{symbol}/{from}/{to}")]
        public async Task<IActionResult> GetTrades(string symbol, DateTime from, DateTime to)
        {
            var value = await _marketService.GetDataInRange(symbol.Replace("_", ""), from, to);
            return Ok(value);
        }

    }
}
