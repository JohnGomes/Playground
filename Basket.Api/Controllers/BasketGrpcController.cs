using System;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketGrpcController : Controller
    {
        // GET
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(new Object());
        }
    }
}