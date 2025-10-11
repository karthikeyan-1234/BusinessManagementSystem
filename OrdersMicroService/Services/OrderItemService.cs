using CommonServices.Events;
using CommonServices.Models;

using Confluent.Kafka;

using OrdersMicroService.Repositories;

using System.Collections.Immutable;
using System.Text.Json;

namespace OrdersMicroService.Services
{
    public class OrderItemService : IOrderItemService, IHostedService
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory; // Use Scope Factory to resolve scoped services

        public OrderItemService(IProducer<string, string> producer, IConsumer<string, string> consumer, IServiceScopeFactory serviceScopeFactory)
        {
            _producer = producer;
            _consumer = consumer;
            _serviceScopeFactory = serviceScopeFactory;

        }

        public async Task<string> CreateOrderItemAsync(OrderItem newOrder)
        {
            var orderId = Guid.NewGuid();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IOrderItemRepository _orderRepository = scope.ServiceProvider.GetRequiredService<IOrderItemRepository>();
                newOrder.OrderId = Guid.NewGuid();
                var order = await _orderRepository.SaveAsync(newOrder);

                // Publish "OrderCreated" event to Kafka
                var evt = new OrderCreatedEvent
                (
                    newOrder.OrderId,
                    newOrder.Price,
                    newOrder.ProductId,
                    newOrder.Quantity
                );


                await _producer.ProduceAsync("order-created", new Message<string, string> //[$$$ 1] Order Created
                {
                    Key = newOrder.OrderId.ToString(),
                    Value = JsonSerializer.Serialize(evt)
                });

                Console.WriteLine($"[OrderSaga] Published OrderCreated for {newOrder.OrderId}");

                return newOrder.OrderId.ToString();
            }
        }

        //Delete order async

         public async Task<bool> DeleteOrderItemAsync(Guid OrderId)
        {
            var orderId = Guid.NewGuid();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    IOrderItemRepository _orderRepository = scope.ServiceProvider.GetRequiredService<IOrderItemRepository>();
                    var delOrder = await _orderRepository.GetAsync(OrderId);

                    // Publish "OrderDeleted" event to Kafka
                    var evt = new OrderCancelledEvent(delOrder);

                    await _producer.ProduceAsync("order-deleted", new Message<string, string>
                    {
                        Key = delOrder.OrderId.ToString(),
                        Value = JsonSerializer.Serialize(evt)
                    });

                    Console.WriteLine($"[OrderSaga] Published OrderDeleted for {delOrder.OrderId}");



                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[OrderSaga] Error deleting order: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<OrderItem?> GetOrderItemByIdAsync(Guid id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IOrderItemRepository _orderRepository = scope.ServiceProvider.GetRequiredService<IOrderItemRepository>();
            return await _orderRepository.GetAsync(id);
        }

        // This runs in a background worker service

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start listening for events in a background task
            Task.Run(() => ListenForEvents(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close(); // Ensure the consumer leaves the group cleanly.
            return Task.CompletedTask;
        }

        public async Task ListenForEvents(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(new[] { "payment-processed", "payment-failed", "inventory-reserved", "inventory-failed" });

            while (!cancellationToken.IsCancellationRequested)
            {
                var cr = _consumer.Consume(cancellationToken);

                switch (cr.Topic)
                {
                     case "payment-processed":  //[$$$ 4] Payment Processed
                        await HandlePaymentProcessed(cr.Message.Value);
                        break;

                     case "inventory-reserved":
                        await HandleInventoryReserved(cr.Message.Value);    //[$$$ 8] Inventory Reserved
                        break;

                    case "payment-failed":
                        await HandlePaymentFailed(cr.Message.Value);  //[### 4] Payment Failed
                        break;
                    case "inventory-failed":
                        await HandleInventoryFailed(cr.Message.Value);   //[### 8] Inventory Failed
                        break;
                }
            }
        }

        private async Task HandlePaymentProcessed(string message)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IOrderItemRepository _orderRepository = scope.ServiceProvider.GetRequiredService<IOrderItemRepository>();
            var evt = JsonSerializer.Deserialize<PaymentProcessedEvent>(message)!;
            var order = await _orderRepository.GetAsync(evt.OrderId);

            

            // Now request inventory reservation
            await _producer.ProduceAsync("reserve-inventory", new Message<string, string>   //[$$$ 5] Reserve Inventory
            {
                Key = order.OrderId.ToString(),
                Value = JsonSerializer.Serialize(new ReserveInventory(order.OrderId,order.ProductId,evt.qty))
            });

            Console.WriteLine($"[OrderSaga] Payment processed, requesting inventory for {order.OrderId}");
        }

        private async Task HandlePaymentFailed(string message)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IOrderItemRepository _orderRepository = scope.ServiceProvider.GetRequiredService<IOrderItemRepository>();
            var evt = JsonSerializer.Deserialize<PaymentFailedEvent>(message)!;
            var order = await _orderRepository.GetAsync(evt.OrderId);
            if (order == null) return;

            order.Status = OrderStatus.Failed;
            await _orderRepository.UpdateAsync(order);
            Console.WriteLine($"[OrderSaga] Order {order.OrderId} failed due to payment error.");
        }

        private async Task HandleInventoryReserved(string message)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IOrderItemRepository _orderRepository = scope.ServiceProvider.GetRequiredService<IOrderItemRepository>();
            var evt = JsonSerializer.Deserialize<InventoryReservedEvent>(message)!;
            var order = await _orderRepository.GetAsync(evt.OrderId);
            if (order == null) return;

            order.Status = OrderStatus.Completed;
            await _orderRepository.UpdateAsync(order);
            Console.WriteLine($"[OrderSaga] Order {order.OrderId} completed successfully.");
        }

        private async Task HandleInventoryFailed(string message)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IOrderItemRepository _orderRepository = scope.ServiceProvider.GetRequiredService<IOrderItemRepository>();
            var evt = JsonSerializer.Deserialize<InventoryFailedEvent>(message)!;
            var order = await _orderRepository.GetAsync(evt.OrderId);
            if (order == null) return;

            // Compensation: publish refund command
            await _producer.ProduceAsync("refund-payment", new Message<string, string>
            {
                Key = order.OrderId.ToString(),
                Value = JsonSerializer.Serialize(new { evt.OrderId })
            });

            order.Status = OrderStatus.Failed;
            await _orderRepository.UpdateAsync(order);

            Console.WriteLine($"[OrderSaga] Order {order.OrderId} failed, refund triggered.");
        }
    
        public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IOrderItemRepository _orderRepository = scope.ServiceProvider.GetRequiredService<IOrderItemRepository>();
            return await _orderRepository.GetAllAsync();
        }

        public async Task<IEnumerable<OrderItem>> CreateMultipleOrderItemsAsync(IEnumerable<OrderItem> newOrders)
        {
            //add multiple order items to an order
            var scope = _serviceScopeFactory.CreateScope();
            IOrderItemRepository _orderRepository = scope.ServiceProvider.GetRequiredService<IOrderItemRepository>();
            await _orderRepository.SaveAsync(newOrders);
            return newOrders;

        }
    }
}
