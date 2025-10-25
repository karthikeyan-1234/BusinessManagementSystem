using CommonServices.Auth;
using CommonServices.Models;
using CommonServices.Repositories;

using Confluent.Kafka;

using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

using PaymentsMicroService.Contexts;
using PaymentsMicroService.Services;

using PaymentTransactionsMicroService.Services;

using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<KeycloakAuthorizationFilter>();


// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<KeycloakAuthorizationFilter>();
});

builder.Services.AddHttpClient();


// 🔑 Configure Keycloak Authentication (uses appsettings.json)
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
// 🛡️ Configure Keycloak Authorization Services
builder.Services.AddKeycloakAuthorization(builder.Configuration);

// ⚠️ NECESSARY CUSTOM LOGIC: Fix HTTPS requirement and add custom token validation
builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    // CRITICAL: Disable HTTPS requirement for development
    options.RequireHttpsMetadata = false;

    // Required for multiple audience support
    options.TokenValidationParameters.ValidAudiences = new[] { "api-app", "angular-app", "master-realm", "account" };
    options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;

    // Required for custom azp validation and role extraction
    options.Events = new JwtBearerEvents
    {

        OnTokenValidated = ctx =>
        {
            // Reject if token doesn't contain UMA "authorization.permissions"
            var hasRpt = ctx.Principal?.Claims.Any(c => c.Type == "authorization") ?? false;
            if (!hasRpt)
            {
                ctx.Fail("RPT (UMA token) required.");
            }
            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            Console.WriteLine("OnChallenge: " + context.Error + " - " + context.ErrorDescription);
            return Task.CompletedTask;
        }
    };
});



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
