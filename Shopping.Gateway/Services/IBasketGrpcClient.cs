using System.Threading.Tasks;
using GrpcBasket;
using Shopping.Gateway.Models;

 namespace Shopping.Gateway.Services
{
    public interface IBasketGrpcClient
    {
        Task<CatalogItem> GetCatalogItem(int id);
        Task<HelloReply> SayHello();
        // Task<CatalogItem> GetCatalogItem(int id);
    }
}
