using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradeDeskBroker.Market;
using TradeDeskData;

namespace TradeDeskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        IFinancialRepository _financeRepo;
        IMarketService _marketService;
        public DataController(IFinancialRepository financeRepo, IMarketService marketService)
        {
            _financeRepo = financeRepo;
            _marketService = marketService;
        }

        [HttpGet]
        [Route("searchTrades/{symbol}/{dateTime}")]
        [ResponseCache(Duration = 600)]
        public async Task<IActionResult> SearchTradeClosestTo(string symbol, DateTime dateTime)
        {
            var value = await _financeRepo.GetClosestTradeAfter(symbol, dateTime);
            return Ok(value);
        }

        [HttpGet]
        [Route("latestTrades/{symbol}/{length}")]
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> GetLatestTrades(string symbol, int length)
        {
            var data = await _financeRepo.GetLastNRecordsForSymbolAsync(symbol.Replace("_USDT", "USDT"), length);
            return Ok(data);
        }

        [HttpGet]
        [Route("latestTradesForId/{symbol}/{id}")]
        [ResponseCache(Duration = 10)]
        public async Task<IActionResult> GetLatestTradesForId(string symbol, int id)
        {
            var data = await _financeRepo.GetRecordsAfterId(symbol.Replace("_USDT", "USDT"), id);
            return Ok(data);
        }

        [HttpGet]
        [Route("latestTradesForIdCount/{symbol}/{id}/{count}")]
        [ResponseCache(Duration = 600)]
        public async Task<IActionResult> GetLatestTradesForId(string symbol, int id, int count)
        {
            var data = await _financeRepo.GetRecordsAfterIdCount(symbol.Replace("_USDT", "USDT"), id, count);
            return Ok(data);
        }
    }
}
