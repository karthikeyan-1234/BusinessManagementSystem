using CommonServices.Models;

using Confluent.Kafka;

using Microsoft.AspNetCore.Mvc; // for FromServices attribute
using Microsoft.EntityFrameworkCore;

using NotificationService;
using NotificationService.Contexts;
using NotificationService.Hubs;
using NotificationService.Services;

using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379", options => {
        options.Configuration.ChannelPrefix = "notificationhub";
    });

builder.Services.AddDbContext<NotificationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("NotificationsDbConnection")));

// Kafka consumer hosted service
builder.Services.AddHostedService<KafkaConsumerService>();

// Kafka Consumer
builder.Services.AddSingleton<IConsumer<Ignore, string>>(sp =>
{
    var config = new ConsumerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
        GroupId = "order-service-group",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };
    return new ConsumerBuilder<Ignore, string>(config).Build();
});

// repository
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

//cache
// Redis connection registration
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis")
                        ?? "localhost:6379"; // fallback
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddScoped<IRedisNotificationCache, RedisNotificationCache>();

//CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.MapHub<NotificationHub>("/notificationHub");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
