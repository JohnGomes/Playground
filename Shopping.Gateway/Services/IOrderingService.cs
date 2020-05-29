using System.Threading.Tasks;
using Shopping.Gateway.Models;

namespace Shopping.Gateway.Services
{
    public interface IOrderingService
    {
        Task<OrderData> GetOrderDraftAsync(BasketData basketData);
    }
}