using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using GrpcBasket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shopping.Gateway.Config;
using Shopping.Gateway.Models; // using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;

namespace Shopping.Gateway.Services
{
    public class BasketGrpcClient : IBasketGrpcClient
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<BasketApiClient> _logger;
        private readonly UrlsConfig _urls;

        public BasketGrpcClient(HttpClient httpClient, ILogger<BasketApiClient> logger, IOptions<UrlsConfig> config)
        {
            _apiClient = httpClient;
            _logger = logger;
            _urls = config.Value;
        }

        public async Task<HelloReply> SayHello()
        {
            return await GrpcService.CallService<HelloReply>(_urls.GrpcBasket,
                async channel => await new GrpcBasket.Basket.BasketClient(channel)
                    .SayHelloAsync(new HelloRequest {Name = "Shopping Gateway"}));
        }


        public async Task<CatalogItem> GetCatalogItem(int id)
        {
            return await GrpcService.CallService(_urls.GrpcBasket, async channel =>
            {
                 var client = new GrpcBasket.Basket.BasketClient(channel);

                var response = await client.GetCatalogItemAsync(new CatalogItemRequest{Id = id});


                var catalogItem = new CatalogItem();

                catalogItem.Id = response.Id;
                catalogItem.Name = response.Name;
                catalogItem.Price = (decimal) response.Price;
                catalogItem.PictureUri = response.PictureUri;

                return catalogItem;
            });
        }
    }
}