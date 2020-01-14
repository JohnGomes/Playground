using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping.Gateway.Models;
using Shopping.Gateway.Services;

namespace Shopping.Gateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketGatewayController : Controller
    {
        private IBasketApiClient _client;

        public BasketGatewayController(IBasketApiClient basketApiClient)
        {
            _client = basketApiClient;
        }
        
        // GET
        [HttpGet]
        public async Task<CatalogItem> Get()
        {
            return await _client.GetCatalogItem(1);
            //var data =
            //var basket = !string.IsNullOrEmpty(data) ? JsonConvert.DeserializeObject<CatalogItem>(data) : null;
            //return data;
        }
    }
}