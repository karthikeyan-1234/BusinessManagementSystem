using CommonServices.Models;
using CommonServices.Repositories;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

using ProductsMicroService.Contexts;
using ProductsMicroService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Product Service
builder.Services.AddScoped<IProductService, ProductService>();

// Register Generic Repository (refactored to <T>)
builder.Services.AddScoped(typeof(IGenericRepository<Product>), typeof(GenericRepository<ProductsDbContext,Product>));

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
        GroupId = "products-service-group",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };
    return new ConsumerBuilder<string, string>(config).Build();
});

// Database Context (switch between SQL Server and InMemory for testing)
builder.Services.AddDbContext<ProductsDbContext>(opt =>
    // opt.UseInMemoryDatabase("ProductsDb")
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ProductsDbConnection"))
);

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
