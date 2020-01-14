using System;
using Microsoft.AspNetCore.Mvc;

namespace Shopping.Gateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketGrpcGatewayController : Controller
    {
        // GET
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(new Object());
        }
    }
}