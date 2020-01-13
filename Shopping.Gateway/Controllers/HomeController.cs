using Microsoft.AspNetCore.Mvc;

namespace Shopping.Gateway.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : Controller
    {
        // GET
        [HttpGet]
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}