using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace UserAccount.Api.Infrastructure.Messaging;

public class ServiceBusPublisher(ServiceBusClient client)
{
    public async Task PublishAsync<T>(string topicName, T message, CancellationToken ct = default)
        => await PublishAsync(topicName, message, properties: null, ct);

    public async Task PublishAsync<T>(string topicName, T message, IDictionary<string, object>? properties, CancellationToken ct = default)
    {
        var sender = client.CreateSender(topicName);
        await using (sender)
        {
            var json = JsonSerializer.Serialize(message);
            var sbMessage = new ServiceBusMessage(json)
            {
                ContentType = "application/json"
            };

            if (properties is not null)
                foreach (var (key, value) in properties)
                    sbMessage.ApplicationProperties[key] = value;

            await sender.SendMessageAsync(sbMessage, ct);
        }
    }
}
