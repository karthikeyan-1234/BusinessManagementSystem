using Confluent.Kafka;

using Microsoft.EntityFrameworkCore;

using InventoryMicroService.Contexts;
using InventoryMicroService.Repositories;
using InventoryMicroService.Services;
using PaymentsMicroService.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();



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
builder.Services.AddDbContext<InventoryDbContext>(
    //static opt =>    opt.UseInMemoryDatabase("InventoryDb");
opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("InventoryDbConnection"))
);

builder.Services.AddHostedService<InventoryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
