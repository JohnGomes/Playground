using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcBasket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Basket.Api.Grpc
{
    public class BasketGrpcService : GrpcBasket.Basket.BasketBase
    {
        //TODO
        // private readonly IBasketRepository _repository;
        private readonly ILogger<BasketGrpcService> _logger;

        public BasketGrpcService( ILogger<BasketGrpcService> logger)
        {
            // _repository = repository;
            _logger = logger;
        }
        
        [AllowAnonymous]
        public override async Task<CatalogItemRequest> Get(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call from method {Method} for basket id {Id}", context.Method);

            //var data = await _repository.GetBasketAsync(request.Id);

            // if (data != null)
            // {
            //     context.Status = new Status(StatusCode.OK, $"Basket with id {request.Id} do exist");
            //
            //     return MapToCustomerBasketResponse(data);
            // }
            // else
            // {
            //     context.Status = new Status(StatusCode.NotFound, $"Basket with id {request.Id} do not exist");
            // }

            return new CatalogItemRequest();
        }
        
        [AllowAnonymous]
        public override async Task<CustomerBasketResponse> GetBasketById(BasketRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call from method {Method} for basket id {Id}", context.Method, request.Id);

            //var data = await _repository.GetBasketAsync(request.Id);

            // if (data != null)
            // {
            //     context.Status = new Status(StatusCode.OK, $"Basket with id {request.Id} do exist");
            //
            //     return MapToCustomerBasketResponse(data);
            // }
            // else
            // {
            //     context.Status = new Status(StatusCode.NotFound, $"Basket with id {request.Id} do not exist");
            // }

            return new CustomerBasketResponse();
        }

        // [AllowAnonymous]
        // public override async Task<CustomerBasketResponse> GetBasketById(BasketRequest request, ServerCallContext context)
        // {
        //     _logger.LogInformation("Begin grpc call from method {Method} for basket id {Id}", context.Method, request.Id);
        //
        //     var data = await _repository.GetBasketAsync(request.Id);
        //
        //     if (data != null)
        //     {
        //         context.Status = new Status(StatusCode.OK, $"Basket with id {request.Id} do exist");
        //
        //         return MapToCustomerBasketResponse(data);
        //     }
        //     else
        //     {
        //         context.Status = new Status(StatusCode.NotFound, $"Basket with id {request.Id} do not exist");
        //     }
        //
        //     return new CustomerBasketResponse();
        // }

    }
}
