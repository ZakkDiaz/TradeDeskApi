using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradeDeskData;

namespace TradeDeskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        IFinancialRepository _financeRepo;
        public DataController(IFinancialRepository financeRepo)
        {
            _financeRepo = financeRepo;
        }
        [HttpGet]
        [Route("latestTrades/{symbol}")]
        public async Task<IActionResult> GetLatestTrades(string symbol)
        {
            return Ok(await _financeRepo.GetLastNRecordsForSymbolAsync(symbol.Replace("_USDT", "USDT"), 10000));
        }
    }
}
