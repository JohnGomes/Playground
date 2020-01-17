using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcBasket;
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

        [HttpGet]
        [Route("{id}")]
        public async Task<CatalogItem> Get([FromQuery]int id = 1) => await _basketGrpcClient.GetCatalogItem(id);

        [HttpGet]
        public async Task<string> SayHello() => (await _basketGrpcClient.SayHello()).Message;
    }
}