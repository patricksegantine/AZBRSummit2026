using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using UserAccount.Api.Features.Accounts.CreateAccount;
using UserAccount.Api.Infrastructure.Messaging;
using UserAccount.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

builder.Services.AddSingleton(_ =>
    new ServiceBusClient(builder.Configuration.GetConnectionString("AzureServiceBus")));

builder.Services.AddScoped<ServiceBusPublisher>();
builder.Services.AddScoped<CreateAccountHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapCreateAccount();

app.Run();
