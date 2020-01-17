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
    }
}
