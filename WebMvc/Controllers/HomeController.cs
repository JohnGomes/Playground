using Microsoft.AspNetCore.Mvc;

namespace Microsoft.eShopContainers.WebMvc.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    // [Route("")]
    public class HomeController : Controller
    {
        // GET
        [HttpGet]
        public IActionResult Index()
        {
            return new RedirectResult("~/catalog");
        }
    }
}