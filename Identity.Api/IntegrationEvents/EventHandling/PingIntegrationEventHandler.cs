using System.Threading.Tasks;
using EventBus.Abstractions;
using Identity.Api.IntegrationEvents.Events;

namespace Identity.Api.IntegrationEvents.EventHandling
{
    public class PingIntegrationEventHandler : IIntegrationEventHandler<PingIntegrationEvent>
    {
        private readonly Serilog.ILogger _logger;

        public PingIntegrationEventHandler(Serilog.ILogger logger)
        {
            _logger = logger;
        }
        public async Task Handle(PingIntegrationEvent @event)
        {
            System.Console.WriteLine($"----- Handling PING integration event: - ({@event})");
            _logger.Information("----- Handling PING integration event: - ({@IntegrationEvent})", @event);
        }
    }
}


 // public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler : 
 //        IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
 //    {
 //        private readonly CatalogContext _catalogContext;
 //        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;
 //        private readonly ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> _logger;
 //
 //        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(
 //            CatalogContext catalogContext,
 //            ICatalogIntegrationEventService catalogIntegrationEventService,
 //            ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger)
 //        {
 //            _catalogContext = catalogContext;
 //            _catalogIntegrationEventService = catalogIntegrationEventService;
 //            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
 //        }
 //
 //        public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event)
 //        {
 //            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
 //            {
 //                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
 //
 //                var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();
 //
 //                foreach (var orderStockItem in @event.OrderStockItems)
 //                {
 //                    var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);
 //                    var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
 //                    var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, hasStock);
 //
 //                    confirmedOrderStockItems.Add(confirmedOrderStockItem);
 //                }
 //
 //                var confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => !c.HasStock)
 //                    ? (IntegrationEvent)new OrderStockRejectedIntegrationEvent(@event.OrderId, confirmedOrderStockItems)
 //                    : new OrderStockConfirmedIntegrationEvent(@event.OrderId);
 //
 //                await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(confirmedIntegrationEvent);
 //                await _catalogIntegrationEventService.PublishThroughEventBusAsync(confirmedIntegrationEvent);
 //
 //            }
 //        }
 //    }