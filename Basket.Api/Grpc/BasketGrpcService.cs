using System.Linq;
using System.Threading.Tasks;
using Basket.Api.Model;
using Grpc.Core;
using GrpcBasket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Basket.Api.Grpc
{
    public class BasketGrpcService : GrpcBasket.Basket.BasketBase
    {
         private readonly IBasketRepository _repository;
        private readonly ILogger<BasketGrpcService> _logger;

        public BasketGrpcService(IBasketRepository repository, ILogger<BasketGrpcService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name + " From Basket Api"
            });
        }

        public override Task<CatalogItemReply> GetCatalogItem(CatalogItemRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CatalogItemReply
            {
                Id = 1,
                Name = "item",
                PictureUri = "123",
                Price = 10.99
            });
        }
        
        [AllowAnonymous]
        public override async Task<CustomerBasketResponse> GetBasketById(BasketRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call from method {Method} for basket id {Id}", context.Method, request.Id);

            var data = await _repository.GetBasketAsync(request.Id);

            if (data != null)
            {
                context.Status = new Status(StatusCode.OK, $"Basket with id {request.Id} do exist");

                return MapToCustomerBasketResponse(data);
            }
            else
            {
                context.Status = new Status(StatusCode.NotFound, $"Basket with id {request.Id} do not exist");
            }

            return new CustomerBasketResponse();
        }

        public override async Task<CustomerBasketResponse> UpdateBasket(CustomerBasketRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call BasketService.UpdateBasketAsync for buyer id {Buyerid}", request.Buyerid);

            var customerBasket = MapToCustomerBasket(request);

            var response = await _repository.UpdateBasketAsync(customerBasket);

            if (response != null)
            {
                return MapToCustomerBasketResponse(response);
            }

            context.Status = new Status(StatusCode.NotFound, $"Basket with buyer id {request.Buyerid} do not exist");

            return null;
        }

        private CustomerBasketResponse MapToCustomerBasketResponse(CustomerBasket customerBasket)
        {
            var response = new CustomerBasketResponse
            {
                Buyerid = customerBasket.BuyerId
            };

            customerBasket.Items.ForEach(item => response.Items.Add(new BasketItemResponse
            {
                Id = item.Id,
                Oldunitprice = (double)item.OldUnitPrice,
                Pictureurl = item.PictureUrl,
                Productid = item.ProductId,
                Productname = item.ProductName,
                Quantity = item.Quantity,
                Unitprice = (double)item.UnitPrice
            }));

            return response;
        }

        private CustomerBasket MapToCustomerBasket(CustomerBasketRequest customerBasketRequest)
        {
            var response = new CustomerBasket
            {
                BuyerId = customerBasketRequest.Buyerid
            };

            customerBasketRequest.Items.ToList().ForEach(item => response.Items.Add(new BasketItem
            {
                Id = item.Id,
                OldUnitPrice = (decimal)item.Oldunitprice,
                PictureUrl = item.Pictureurl,
                ProductId = item.Productid,
                ProductName = item.Productname,
                Quantity = item.Quantity,
                UnitPrice = (decimal)item.Unitprice
            }));

            return response;
        }
    }
}
