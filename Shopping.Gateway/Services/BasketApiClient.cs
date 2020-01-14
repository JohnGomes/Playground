using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shopping.Gateway.Config;
using Shopping.Gateway.Models; // using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;

namespace Shopping.Gateway.Services
{
    public class BasketApiClient : IBasketApiClient
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<BasketApiClient> _logger;
        private readonly UrlsConfig _urls;

        public BasketApiClient(HttpClient httpClient, ILogger<BasketApiClient> logger, IOptions<UrlsConfig> config)
        {
            _apiClient = httpClient;
            _logger = logger;
            _urls = config.Value;
        }

        public async Task<CatalogItem> GetCatalogItem(int id)
        {

            var url = _urls.Basket + UrlsConfig.BasketOperations.GetItem();
            var response = await _apiClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var ordersDraftResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CatalogItem>(ordersDraftResponse);
        }
    }
}
