
using CommonServices.Models;

using Confluent.Kafka;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

using NotificationService.Contexts;
using NotificationService.Hubs;

using System.Text.Json;

namespace NotificationService.Services
{
    public class KafkaConsumerService : IHostedService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IServiceProvider _sp;
        private readonly string _topic;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConsumer<Ignore, string> consumer, IServiceProvider sp, IConfiguration cfg)
        {
            this._logger = logger;
            _consumer = consumer;
            _sp = sp;
            _topic = "notifications";
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(new[] { _topic });
            Task.Run(() => ListenForEvents(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        private async Task ListenForEvents(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(cancellationToken);

                    var raw = cr.Message.Value;

                    using var scope = _sp.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                    var cache = scope.ServiceProvider.GetRequiredService<IRedisNotificationCache>();
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();



                    switch (cr.Topic) {
                        case "notifications":
                            var dto = JsonSerializer.Deserialize<NotificationDto>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            if (dto == null) continue;

                            var entity = new Notification()
                            {
                                Id = dto.Id,
                                UserId = dto.UserId,
                                Title = dto.Title,
                                Message = dto.Message,
                                Link = dto.Link,
                                CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
                                IsRead = false
                            };

                            await dbContext.Notifications.AddAsync(entity, cancellationToken);
                            await dbContext.SaveChangesAsync(cancellationToken);

                            var dtoToCache = new NotificationDto
                            {
                                Id = entity.Id,
                                UserId = entity.UserId,
                                Title = entity.Title,
                                Message = entity.Message,
                                Link = entity.Link,
                                CreatedAt = entity.CreatedAt
                            };

                            if (!string.IsNullOrEmpty(entity.UserId))
                            {
                                await cache.PushUnreadAsync(dtoToCache);
                                await hub.Clients.Group($"user-{entity.UserId}")
                                         .SendAsync("ReceiveNotification", dtoToCache, cancellationToken: cancellationToken);
                            }
                            else
                            {
                                await hub.Clients.All.SendAsync("ReceiveNotification", dtoToCache, cancellationToken: cancellationToken);
                            }


                            break;
                    }
                }
                catch (OperationCanceledException) { /* shutting down */ }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Kafka consumer loop");
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            return Task.CompletedTask;
        }

    }
}
