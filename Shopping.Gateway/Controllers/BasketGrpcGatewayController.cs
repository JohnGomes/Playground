using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shopping.Gateway.Models;
using Shopping.Gateway.Services;

namespace Shopping.Gateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketGrpcGatewayController : Controller
    {
        private IBasketGrpcClient _basketGrpcClient;

        public BasketGrpcGatewayController(IBasketGrpcClient basketGrpcClient)
        {
            _basketGrpcClient = basketGrpcClient;
        }
        // GET
        [HttpGet]
        public async Task<CatalogItem> Get()
        {
            // return Ok(new Object());
            
            return await _basketGrpcClient.GetCatalogItem();
        }
    }
}