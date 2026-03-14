using Azure.Messaging.ServiceBus;
using Mission.MgmCompleteAnalyzer.Worker.Handlers;
using Mission.MgmCompleteAnalyzer.Worker.Messaging;
using System.Text.Json;

namespace Mission.MgmCompleteAnalyzer.Worker;

public class Worker(
    ServiceBusProcessor processor,
    MgmCompletionHandler completionHandler,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        processor.ProcessMessageAsync += OnMessageReceivedAsync;
        processor.ProcessErrorAsync += OnErrorAsync;

        await processor.StartProcessingAsync(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

        await processor.StopProcessingAsync(stoppingToken);
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

        // MissionId and IndicationToken are guaranteed by the Service Bus subscription filter,
        // but we validate defensively in case the message arrives via another path.
        args.Message.ApplicationProperties.TryGetValue("MissionId", out var missionIdRaw);
        Guid.TryParse(missionIdRaw?.ToString(), out var missionId);

        if (!args.Message.ApplicationProperties.TryGetValue("IndicationToken", out var indicationTokenRaw)
            || string.IsNullOrWhiteSpace(indicationTokenRaw?.ToString()))
        {
            logger.LogWarning(
                "Message is missing a valid IndicationToken property. AccountId: {AccountId} | MissionId: {MissionId}",
                message.Id, missionId);
            await args.DeadLetterMessageAsync(args.Message, "MissingIndicationToken", "ApplicationProperty 'IndicationToken' is absent or empty.", args.CancellationToken);
            return;
        }

        var command = new MgmCompletionCommand(
            UserId: message.Id,
            UserEmail: message.Email,
            MissionId: missionId,
            IndicationToken: indicationTokenRaw.ToString()!
        );

        var result = completionHandler.Handle(command);

        if (!result.IsSuccess)
        {
            logger.LogWarning(
                "MGM completion failed. AccountId: {AccountId} | MissionId: {MissionId} | Reason: {Reason}",
                message.Id, missionId, result.FailureReason);
            await args.DeadLetterMessageAsync(args.Message, result.FailureCode, result.FailureReason, args.CancellationToken);
            return;
        }

        logger.LogInformation(
            "UserMission {UserMissionId} created: MGM mission {MissionId} completed by user {UserId} | Email: {Email} | IndicationToken: {IndicationToken}",
            result.UserMission!.Id, missionId, message.Id, message.Email, command.IndicationToken);

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
