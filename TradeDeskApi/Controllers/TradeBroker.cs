using Microsoft.AspNetCore.Mvc;
using TradeDeskBroker;

namespace TradeDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeBrokerController : Controller
    {
        IBrokerageService _brokerageService;
        public TradeBrokerController(IBrokerageService brokerageService)
        {
            _brokerageService = brokerageService;
        }

        [HttpGet]
        [Route("wallet")]
        public IActionResult GetWallet()
        {
            return Ok(_brokerageService.GetWallet(User.Identity.Name));
        }

        [HttpGet]
        [Route("AddFunds/{funds}")]
        public IActionResult AddFunds(decimal funds)
        {
            _brokerageService.AddFunds(User.Identity.Name, funds);
            return GetWallet();
        }
    }
}
