using CommonServices.Models;
using CommonServices.Repositories;

using Confluent.Kafka;

using Microsoft.EntityFrameworkCore;

using PaymentsMicroService.Contexts;
using PaymentsMicroService.Services;

using PaymentTransactionsMicroService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();

//Add generic repository
builder.Services.AddScoped(typeof(IGenericRepository<PaymentTransaction>), typeof(GenericRepository<PaymentsDbContext,PaymentTransaction>));



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
// Ensure the Microsoft.EntityFrameworkCore.InMemory package is installed in your project
builder.Services.AddDbContext<PaymentsDbContext>(
    //static opt =>    opt.UseInMemoryDatabase("PaymentsDb");
opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PaymentsDbConnection"))
);


builder.Services.AddHostedService<PaymentTransactionService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<StripeOptions>()
    .Bind(builder.Configuration.GetSection("Stripe"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
