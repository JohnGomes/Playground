using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Services.ModelDTOs;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface IBasketService
    {
        Task<ViewModels.Basket> GetBasket(ApplicationUser user);
        Task AddItemToBasket(ApplicationUser user, int productId);
        Task<ViewModels.Basket> UpdateBasket(ViewModels.Basket basket);
        Task Checkout(BasketDTO basket);
        Task<ViewModels.Basket> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities);
        Task<Order> GetOrderDraft(string basketId);
    }
}
