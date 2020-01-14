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

        public async Task<CatalogItem> GetCatalogItem()
        {
            return await GrpcService.CallService(_urls.GrpcBasket, async channel =>
            {
                // var client = new Basket.BasketClient(channel);
                // _logger.LogDebug("grpc client created, request = {@id}");
                // var response = new CatalogItemRequest();
                // try
                // {
                //      response = await client.GetAsync(new Empty());
                // }
                // catch (Exception e)
                // {
                //     _logger.LogError(e.Message);
                // }
                //
                // _logger.LogDebug("grpc response {@response}", response);


                var catalogItem = new CatalogItem();

                // catalogItem.Id = response.Id;
                // catalogItem.Name = response.Name;
                // catalogItem.Price = (decimal) response.Price;
                // catalogItem.PictureUri = response.Pictureuri;

                return catalogItem;
            });
        }
    }
}