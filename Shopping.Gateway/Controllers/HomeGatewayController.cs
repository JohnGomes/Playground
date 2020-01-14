using Microsoft.AspNetCore.Mvc;

namespace Shopping.Gateway.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("")]
    public class HomeGatewayController : Controller
    {
        // GET
        [HttpGet]
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}