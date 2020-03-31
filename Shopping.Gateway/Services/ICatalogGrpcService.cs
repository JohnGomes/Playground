using System.Collections.Generic;
using System.Threading.Tasks;
using Shopping.Gateway.Models;

namespace Shopping.Gateway.Services
{
    public interface ICatalogGrpcService
    {
        Task<CatalogItem> GetCatalogItemAsync(int id);

        Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids);
    }
}
