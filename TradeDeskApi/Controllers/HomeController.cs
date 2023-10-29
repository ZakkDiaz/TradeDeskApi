using Microsoft.AspNetCore.Mvc;
using TradeDeskTop.Models;

namespace TradeDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new UserProfile() {  Name = User.Identity.Name });
        }
    }
}
