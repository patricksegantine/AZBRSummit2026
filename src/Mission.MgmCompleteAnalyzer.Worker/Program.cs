using Azure.Messaging.ServiceBus;
using Mission.MgmCompleteAnalyzer.Worker;
using Mission.MgmCompleteAnalyzer.Worker.Handlers;
using Mission.Infrastructure.Data;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AzureServiceBus")!;
var queueName = builder.Configuration["ServiceBus:QueueName"]!;

builder.Services.AddSingleton(_ => new ServiceBusClient(connectionString));

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<ServiceBusClient>();
    return client.CreateProcessor(queueName, new ServiceBusProcessorOptions
    {
        MaxConcurrentCalls = 1,
        AutoCompleteMessages = false
    });
});

builder.Services.AddSingleton<MissionStore>();
builder.Services.AddSingleton<UserMissionStore>();
builder.Services.AddSingleton<IndicationTokenStore>();
builder.Services.AddSingleton<MgmCompletionHandler>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
