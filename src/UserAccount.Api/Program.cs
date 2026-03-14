using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using UserAccount.Api.Features.Accounts.CreateAccount;
using UserAccount.Api.Features.Accounts.GetAccount;
using UserAccount.Api.Features.Accounts.UpdateAccount;
using UserAccount.Api.Infrastructure.Messaging;
using UserAccount.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

if (builder.Environment.IsDevelopment())
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("UserAccountDb"));
else
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

builder.Services.AddSingleton(_ =>
    new ServiceBusClient(builder.Configuration.GetConnectionString("AzureServiceBus")));

builder.Services.AddScoped<ServiceBusPublisher>();
builder.Services.AddScoped<CreateAccountHandler>();
builder.Services.AddScoped<GetAccountsHandler>();
builder.Services.AddScoped<UpdateAccountHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapCreateAccount();
app.MapGetAccounts();
app.MapUpdateAccount();

app.Run();
