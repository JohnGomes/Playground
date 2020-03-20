using System.Threading.Tasks;
using Shopping.Gateway.Models;

namespace Shopping.Gateway.Services
{
    public interface IBasketService
    {
        Task<BasketData> GetById(string id);

        Task UpdateAsync(BasketData currentBasket);
    }
}
