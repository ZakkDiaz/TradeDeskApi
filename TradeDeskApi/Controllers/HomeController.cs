using Microsoft.AspNetCore.Mvc;
using TradeDeskTop.Models;

namespace TradeDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet]
        [HttpPost]
        public IActionResult Index() => Ok(new UserProfile() { Name = User.Identity.Name, Id = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value) });
    }
}
