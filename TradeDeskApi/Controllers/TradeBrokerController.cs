using Microsoft.AspNetCore.Mvc;
using TradeDeskApi.Requests;
using TradeDeskBroker;

namespace TradeDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeBrokerController : BaseController
    {
        IBrokerageService _brokerageService;
        public TradeBrokerController(IBrokerageService brokerageService)
        {
            _brokerageService = brokerageService;
        }

        [HttpGet]
        [Route("wallet")]
        public async Task<IActionResult> GetWallet()
        {
            return Ok(await _brokerageService.GetWallet(GetUserId()));
        }

        [HttpGet]
        [Route("AddFunds/{wallet}/{funds}")]
        public async Task<IActionResult> AddFunds(int wallet, decimal funds)
        {
            await _brokerageService.AddFunds(GetUserId(), wallet, funds);
            return Ok(await GetWallet());
        }

        [HttpGet]
        [Route("GetSymbols")]
        public async Task<IActionResult> GetSymbols()
        {
            return Ok(await _brokerageService.GetTrackedSymbolsAsync());
        }

        [HttpGet]
        [Route("GetWatchedSymbols")]
        public async Task<IActionResult> GetWatchedSymbols()
        {
            return Ok(await _brokerageService.GetWatchedSymbols(GetUserId()));
        }

        [HttpPost]
        [Route("AddWatch")]
        public async Task<IActionResult> AddWatch(Watch watch)
        {
            await _brokerageService.AddWatch(watch.UserProfileId, watch.TrackedSymbolId);
            return Ok();
        }

        [HttpPost]
        [Route("RemoveWatch")]
        public async Task<IActionResult> RemoveWatch(Watch watch)
        {
            await _brokerageService.RemoveWatch(watch.UserProfileId, watch.TrackedSymbolId);
            return Ok();
        }
    }
}
