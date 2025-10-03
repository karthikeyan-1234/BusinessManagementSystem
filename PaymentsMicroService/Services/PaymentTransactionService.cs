using CommonServices.Events;
using CommonServices.Models;
using CommonServices.Repositories;

using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PaymentsMicroService.Services;

using System.Text.Json;

namespace PaymentTransactionsMicroService.Services
{
    public class PaymentTransactionService : IPaymentTransactionService, IHostedService
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Random _random = new();

        public PaymentTransactionService(
            IProducer<string, string> producer,
            IConsumer<string, string> consumer,
            IServiceScopeFactory scopeFactory)
        {
            _producer = producer;
            _consumer = consumer;
            _scopeFactory = scopeFactory;
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

        private async Task ListenForEvents(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(new[] { "order-created", "refund-payment" });

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(cancellationToken);

                    switch (cr.Topic)
                    {
                        case "order-created":
                            await HandleOrderCreated(cr.Message.Value, cancellationToken);
                            break;

                        case "refund-payment":
                            await HandleRefundPayment(cr.Message.Value, cancellationToken);
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    break; // graceful shutdown
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PaymentSaga] Error: {ex.Message}");
                }
            }
        }

        private async Task HandleOrderCreated(string message, CancellationToken cancellationToken)
        {
            var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(message)!;

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IGenericRepository<PaymentTransaction>>();

            // Simulate payment (80% success)
            bool paymentSucceeded = _random.Next(1, 101) <= 80;
            var transaction = new PaymentTransaction { OrderId = evt.OrderId };

            if (paymentSucceeded)
            {
                transaction.Status = PaymentStatus.Processed;
                await repo.AddAsync(transaction, cancellationToken);
                await repo.SaveAsync(cancellationToken);

                var processedEvent = new PaymentProcessedEvent(evt.OrderId, transaction.TransactionId, evt.Quantity);
                await PublishEvent("payment-processed", evt.OrderId, processedEvent, cancellationToken);

                Console.WriteLine($"[PaymentSaga] ✅ Payment succeeded for {evt.OrderId}");
            }
            else
            {
                transaction.Status = PaymentStatus.Failed;
                await repo.AddAsync(transaction, cancellationToken);
                await repo.SaveAsync(cancellationToken);

                var failedEvent = new PaymentFailedEvent(evt.OrderId, "Insufficient funds.", evt.Quantity);
                await PublishEvent("payment-failed", evt.OrderId, failedEvent, cancellationToken);

                Console.WriteLine($"[PaymentSaga] ❌ Payment failed for {evt.OrderId}");
            }
        }

        private async Task HandleRefundPayment(string message, CancellationToken cancellationToken)
        {
            var evt = JsonSerializer.Deserialize<PaymentRefundedEvent>(message)!;

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IGenericRepository<PaymentTransaction>>();

            var transaction = await repo.GetAsync(evt.OrderId, cancellationToken);

            if (transaction != null && transaction.Status == PaymentStatus.Processed)
            {
                transaction.Status = PaymentStatus.Refunded;
                repo.Update(transaction);
                await repo.SaveAsync(cancellationToken);

                Console.WriteLine($"[PaymentSaga] ♻️ Refund processed for {evt.OrderId}");
            }
            else
            {
                Console.WriteLine($"[PaymentSaga] Refund requested but no valid processed transaction found for {evt.OrderId}");
            }
        }

        private async Task PublishEvent<T>(string topic, Guid orderId, T evt, CancellationToken cancellationToken)
        {
            var message = new Message<string, string>
            {
                Key = orderId.ToString(),
                Value = JsonSerializer.Serialize(evt)
            };

            await _producer.ProduceAsync(topic, message, cancellationToken);
        }
    }
}
