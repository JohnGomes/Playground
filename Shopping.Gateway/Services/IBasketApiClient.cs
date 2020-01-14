using System.Threading.Tasks;
 using Shopping.Gateway.Models;

 namespace Shopping.Gateway.Services
{
    public interface IBasketApiClient
    {
        Task<CatalogItem> GetCatalogItem(int id);
    }
}
