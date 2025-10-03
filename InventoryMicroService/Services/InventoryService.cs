using CommonServices.Events;
using CommonServices.Models;

using Confluent.Kafka;

using PaymentsMicroService.Repositories;

using System;
using System.Text.Json;
using System.Transactions;

namespace InventoryMicroService.Services
{
    public class InventoryService : IInventoryService, IHostedService
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Random _random = new Random(); // To simulate inventory success/failure

        public InventoryService(
            IProducer<string, string> producer,
            IConsumer<string, string> consumer,
            IServiceScopeFactory serviceScopeFactory)
        {
            _producer = producer;
            _consumer = consumer;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => ListenForEvents(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            return Task.CompletedTask;
        }

        public async Task ListenForEvents(CancellationToken cancellationToken)
        {
            // Subscribe to the topic this service cares about
            _consumer.Subscribe("reserve-inventory");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(cancellationToken);

                    switch (cr.Topic)
                    {
                        case "reserve-inventory":
                            await HandleReserveInventory(cr.Message.Value);
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[InventorySaga] Error processing message: {ex.Message}");
                }
            }
        }

        public async Task HandleReserveInventory(string message)
        {
            var evt = JsonSerializer.Deserialize<ReserveInventory>(message)!;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var inventoryRepository = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();
                var item = await inventoryRepository.GetByProductIdAsync(evt.ProductId);

                // Simulate inventory processing (e.g., 80% success rate)
                bool inventoryAvailable = item is null ? false : true;             

                if (inventoryAvailable)
                {
                    // Sufficient stock, reserve it
                    item!.AvailableQuantity -= evt.qty;
                    await inventoryRepository.UpdateAsync(item);

                    var reservedEvent = new InventoryReservedEvent(evt.OrderId, evt.ProductId);
                    await _producer.ProduceAsync("inventory-reserved", new Message<string, string>
                    {
                        Key = evt.OrderId.ToString(),
                        Value = JsonSerializer.Serialize(reservedEvent)
                    });
                    Console.WriteLine($"[InventorySaga] Inventory reserved for order {evt.OrderId}");
                }
                else
                {
                    // Insufficient stock, publish failure event
                    var reason = item == null ? "Product not found." : "Insufficient stock.";
                    var failedEvent = new InventoryFailedEvent(evt.OrderId,evt.ProductId, reason);
                    await _producer.ProduceAsync("inventory-failed", new Message<string, string>
                    {
                        Key = evt.OrderId.ToString(),
                        Value = JsonSerializer.Serialize(failedEvent)
                    });
                    Console.WriteLine($"[InventorySaga] Inventory reservation failed for order {evt.OrderId}. Reason: {reason}");
                }
            }
        }

        //Add new inventory
        public async Task<InventoryItem> AddInventory(InventoryItem inventory)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var inventoryRepository = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();
                return await inventoryRepository.AddAsync(inventory);
            }
        }

        //Get all inventory
        public async Task<IEnumerable<InventoryItem>> GetAllInventory()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var inventoryRepository = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();
                return await inventoryRepository.GetAllAsync();
            }
        }

        public async Task<bool> DeleteInventory(InventoryItem inventory)
        {
            using(var scope = _serviceScopeFactory.CreateScope())
            {
                var inventoryRepository = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();
                return await inventoryRepository.DeleteAsync(inventory.Id);
            }
        }
    }
}
