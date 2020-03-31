using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Basket.Api.IntegrationEvents.Events;
using Basket.Api.Model;
using Basket.Api.Models;
using Basket.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace Basket.Api.Controllers
{
    
    [Route("api/v1/[controller]")]
    // [Authorize]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<BasketController> _logger;

        public BasketController(
            ILogger<BasketController> logger,
            IBasketRepository repository,
            IIdentityService identityService,
            IEventBus eventBus)
        {
            _logger = logger;
            _repository = repository;
            _identityService = identityService;
            _eventBus = eventBus;
        }
        
        [HttpGet()]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CustomerBasket>>> GetBasketsAsync(string id)
        {
            var basket = await _repository.GetBasketsAsync();

            return Ok(basket);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
        {
            var basket = await _repository.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody]CustomerBasket value)
        {
            return Ok(await _repository.UpdateBasketAsync(value));
        }

        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CheckoutAsync([FromBody]BasketCheckout basketCheckout, [FromHeader(Name = "x-requestid")] string requestId)
        {
            var userId = _identityService.GetUserIdentity();

            basketCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty) ?
                guid : basketCheckout.RequestId;

            var basket = await _repository.GetBasketAsync(userId);

            if (basket == null)
            {
                return BadRequest();
            }

            var userName = this.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

            var eventMessage = new UserCheckoutAcceptedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street,
                basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName,
                basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basketCheckout.RequestId, basket);

            // Once basket is checkout, sends an integration event to
            // ordering.api to convert basket to order and proceeds with
            // order creation process
            try
            {
                _eventBus.Publish(eventMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.Id, "Program.AppName");

                throw;
            }

            return Accepted();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasketByIdAsync(string id)
        {
            await _repository.DeleteBasketAsync(id);
        }
    }
    
    // [Route("api/v1/[controller]")]
    // [ApiController]
    // public class BasketController : Controller
    // {
    //     // GET
    //     [HttpGet]
    //     public ActionResult Get()
    //     {
    //         return Ok(new CatalogItemRequest
    //         {
    //             Id = RandomizerFactory.GetRandomizer(new FieldOptionsInteger{Max=99, Min=1,UseNullValues = false}).Generate(),
    //             Name = RandomizerFactory.GetRandomizer(new FieldOptionsFullName()).Generate(),
    //             PictureUri = RandomizerFactory.GetRandomizer(new FieldOptionsIPv6Address()).Generate(),
    //             Price = Convert.ToDecimal(RandomizerFactory.GetRandomizer(new FieldOptionsInteger{Max=199, Min=100,UseNullValues = false}).Generate())
    //         });
    //     }
    // }
}