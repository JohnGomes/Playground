using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GrpcCatalog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shopping.Gateway.Config;
using Shopping.Gateway.Models;

namespace Shopping.Gateway.Services
{
    public class CatalogGrpcService : ICatalogGrpcService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogGrpcService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly UrlsConfig _urls;

        public CatalogGrpcService(HttpClient httpClient, ILogger<CatalogGrpcService> logger, IOptions<UrlsConfig> config, ILoggerFactory loggerFactory)
        {
            _httpClient = httpClient;
            _logger = logger;
            _loggerFactory = loggerFactory;
            _urls = config.Value;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            return await GrpcCallerService.CallService(_urls.GrpcCatalog, async channel =>
            {
                var client = new Catalog.CatalogClient(channel);
                var request = new CatalogItemRequest { Id = id };
                _logger.LogInformation("grpc client created, request = {@request}", request);
                var response = await client.GetItemByIdAsync(request);
                _logger.LogInformation("grpc response {@response}", response);
                return MapToCatalogItemResponse(response);
            },_loggerFactory);
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {
            return await GrpcCallerService.CallService(_urls.GrpcCatalog, async channel =>
            {
                var client = new Catalog.CatalogClient(channel);
                var request = new CatalogItemsRequest { Ids = string.Join(",", ids), PageIndex = 1, PageSize = 10 };
                _logger.LogInformation("grpc client created, request = {@request}", request);
                var response = await client.GetItemsByIdsAsync(request);
                _logger.LogInformation("grpc response {@response}", response);
                return response.Data.Select(this.MapToCatalogItemResponse);
            },_loggerFactory);
        }

        private CatalogItem MapToCatalogItemResponse(CatalogItemResponse catalogItemResponse)
        {
            return new CatalogItem
            {
                Id = catalogItemResponse.Id,
                Name = catalogItemResponse.Name,
                PictureUri = catalogItemResponse.PictureUri,
                Price = (decimal)catalogItemResponse.Price
            };
        }
    }
}
