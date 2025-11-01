using CommonServices.Models;
using CommonServices.Repositories;

using Confluent.Kafka;

using Microsoft.EntityFrameworkCore;

using PurchaseMicroService.Contexts;
using PurchaseMicroService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDbContext<PurchaseDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("PurchasesDbConnection")
    ));

builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped(typeof(IGenericRepository<Purchase>), typeof(GenericRepository<PurchaseDbContext, Purchase>));
builder.Services.AddScoped(typeof(IGenericRepository<PurchaseItem>), typeof(GenericRepository<PurchaseDbContext, PurchaseItem>));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");


app.UseAuthorization();

app.MapControllers();

app.Run();
