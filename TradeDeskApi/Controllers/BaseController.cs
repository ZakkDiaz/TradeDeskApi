using Microsoft.AspNetCore.Mvc;

namespace TradeDeskApi.Controllers
{
    public class BaseController : Controller
    {
        internal int GetUserId()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
        }
    }
}
