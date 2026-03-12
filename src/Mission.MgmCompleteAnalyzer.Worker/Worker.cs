using Azure.Messaging.ServiceBus;
using Mission.MgmCompleteAnalyzer.Worker.Infrastructure.Data;
using Mission.MgmCompleteAnalyzer.Worker.Messaging;
using System.Text.Json;

namespace Mission.MgmCompleteAnalyzer.Worker;

public class Worker(
    ServiceBusProcessor processor,
    MissionStore missionStore,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        processor.ProcessMessageAsync += OnMessageReceivedAsync;
        processor.ProcessErrorAsync += OnErrorAsync;

        await processor.StartProcessingAsync(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

        await processor.StopProcessingAsync();
        await processor.DisposeAsync();
    }

    private async Task OnMessageReceivedAsync(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();

        var message = JsonSerializer.Deserialize<UserAccountAddedMessage>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (message is null)
        {
            logger.LogWarning("Received null or undeserializable message. MessageId: {MessageId}", args.Message.MessageId);
            await args.DeadLetterMessageAsync(args.Message, "InvalidPayload", "Message could not be deserialized.", args.CancellationToken);
            return;
        }

        // Both CampaignId and IndicationToken are guaranteed by the Service Bus subscription filter,
        // but we validate defensively in case the message arrives via another path.
        if (!args.Message.ApplicationProperties.TryGetValue("CampaignId", out var campaignIdRaw)
            || !Guid.TryParse(campaignIdRaw?.ToString(), out var campaignId))
        {
            logger.LogWarning(
                "Message is missing a valid CampaignId property. AccountId: {AccountId}",
                message.Id);
            await args.DeadLetterMessageAsync(args.Message, "MissingCampaignId", "ApplicationProperty 'CampaignId' is absent or invalid.", args.CancellationToken);
            return;
        }

        if (!args.Message.ApplicationProperties.TryGetValue("IndicationToken", out var indicationTokenRaw)
            || string.IsNullOrWhiteSpace(indicationTokenRaw?.ToString()))
        {
            logger.LogWarning(
                "Message is missing a valid IndicationToken property. AccountId: {AccountId} | CampaignId: {CampaignId}",
                message.Id, campaignId);
            await args.DeadLetterMessageAsync(args.Message, "MissingIndicationToken", "ApplicationProperty 'IndicationToken' is absent or empty.", args.CancellationToken);
            return;
        }

        var indicationToken = indicationTokenRaw.ToString()!;

        var mission = missionStore.GetByCampaignId(campaignId);
        if (mission is null)
        {
            logger.LogWarning(
                "No active MGM mission found for CampaignId {CampaignId}. AccountId: {AccountId}",
                campaignId, message.Id);
            await args.DeadLetterMessageAsync(args.Message, "MissionNotFound", $"No active MGM mission for campaign '{campaignId}'.", args.CancellationToken);
            return;
        }

        missionStore.Complete(mission.Id);

        logger.LogInformation(
            "MGM mission {MissionId} '{MissionName}' completed. ReferredAccountId: {AccountId} | ReferredEmail: {Email} | CampaignId: {CampaignId} | IndicationToken: {IndicationToken}",
            mission.Id, mission.Name, message.Id, message.Email, campaignId, indicationToken);

        // TODO: dispatch email/push notification to the referrer identified by IndicationToken
        //       congratulating them on completing the MGM mission.
        // Suggested payload: { IndicationToken, CampaignId, MissionId, MissionName, ReferredAccountId, ReferredEmail }
        // Channels: email via SendGrid/Communication Services, push via Azure Notification Hubs.

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private Task OnErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception,
            "Error processing message. Source: {ErrorSource} | EntityPath: {EntityPath}",
            args.ErrorSource, args.EntityPath);

        return Task.CompletedTask;
    }
}
