using System.Threading.Tasks;
 using Shopping.Gateway.Models;

 namespace Shopping.Gateway.Services
{
    public interface IBasketGrpcClient
    {
        Task<CatalogItem> GetCatalogItem();
        // Task<CatalogItem> GetCatalogItem(int id);
    }
}
