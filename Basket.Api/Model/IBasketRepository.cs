using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basket.Api.Model
{
    public interface IBasketRepository
    {
        Task<IEnumerable<CustomerBasket>> GetBasketsAsync();
        Task<CustomerBasket> GetBasketAsync(string customerId);
        IEnumerable<string> GetUsers();
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string id);
    }
}
