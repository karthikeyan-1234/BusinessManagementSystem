using CommonServices.Auth;
using CommonServices.Models;
using CommonServices.Repositories;

using Confluent.Kafka;

using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

using ProductsMicroService.Contexts;
using ProductsMicroService.Services;

using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<KeycloakAuthorizationFilter>();


// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<KeycloakAuthorizationFilter>();
});


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


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
