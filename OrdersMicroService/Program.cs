using OrdersMicroService.Contexts;
using OrdersMicroService.Repositories;
using OrdersMicroService.Services;
using Microsoft.EntityFrameworkCore; // Added this using directive for UseInMemoryDatabase extension method
using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using Microsoft.AspNetCore.Cors.Infrastructure;
using CommonServices.Repositories;
using CommonServices.Models; // Ensure this is included for service configuration

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();


// Kafka Producer
builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
        Acks = Acks.All,                     // wait for leader + replicas
        MessageTimeoutMs = 5000   
    };
    return new ProducerBuilder<string, string>(config).Build();
});


// Kafka Consumer
builder.Services.AddSingleton<IConsumer<string, string>>(sp =>
{
    var config = new ConsumerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
        GroupId = "order-service-group",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };
    return new ConsumerBuilder<string, string>(config).Build();
});


// Ensure the Microsoft.EntityFrameworkCore.InMemory package is installed in your project
builder.Services.AddDbContext<OrderDbContext>(
    //static opt =>    opt.UseInMemoryDatabase("InventoryDb");
opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDbConnection"))
);

builder.Services.AddHostedService<OrderItemService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use cors policy builder to build cors policy
CorsPolicyBuilder corsPolicyBuilder = new CorsPolicyBuilder();
CorsPolicy policy = corsPolicyBuilder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod().Build();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy);
});

// Register Product Service
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

// Register Generic Repository (refactored to <T>)
builder.Services.AddScoped(typeof(IGenericRepository<Order>), typeof(GenericRepository<OrderDbContext,Order>));
builder.Services.AddScoped(typeof(IGenericRepository<OrderItem>), typeof(GenericRepository<OrderDbContext, OrderItem>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
    
