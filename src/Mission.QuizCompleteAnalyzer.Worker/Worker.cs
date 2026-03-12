using Azure.Messaging.ServiceBus;
using Mission.QuizCompleteAnalyzer.Worker.Infrastructure.Data;
using Mission.QuizCompleteAnalyzer.Worker.Messaging;
using System.Text.Json;

namespace Mission.QuizCompleteAnalyzer.Worker;

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

        var message = JsonSerializer.Deserialize<QuizAnsweredMessage>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (message is null)
        {
            logger.LogWarning("Received null or undeserializable message. MessageId: {MessageId}", args.Message.MessageId);
            await args.DeadLetterMessageAsync(args.Message, "InvalidPayload", "Message could not be deserialized.", args.CancellationToken);
            return;
        }

        if (!args.Message.ApplicationProperties.TryGetValue("MissionId", out var missionIdRaw)
            || !Guid.TryParse(missionIdRaw?.ToString(), out var missionId))
        {
            logger.LogWarning(
                "Message has no valid MissionId property. Skipping. QuizId: {QuizId} | UserId: {UserId}",
                message.QuizId, message.UserId);
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }

        var mission = missionStore.GetById(missionId);
        if (mission is null)
        {
            logger.LogWarning(
                "Mission {MissionId} not found. QuizId: {QuizId} | UserId: {UserId}",
                missionId, message.QuizId, message.UserId);
            await args.DeadLetterMessageAsync(args.Message, "MissionNotFound", $"Mission '{missionId}' does not exist.", args.CancellationToken);
            return;
        }

        if (!message.UserGotAward)
        {
            logger.LogInformation(
                "User {UserId} did not reach minimum score for Quiz {QuizId}. Mission {MissionId} remains {Status}. UserScore: {UserScore}/{QuizScore}",
                message.UserId, message.QuizId, missionId, mission.Status, message.UserScore, message.QuizScore);
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            return;
        }

        missionStore.Complete(missionId);

        logger.LogInformation(
            "Mission {MissionId} '{MissionName}' completed for User {UserId}. QuizId: {QuizId} | UserScore: {UserScore}/{QuizScore}",
            missionId, mission.Name, message.UserId, message.QuizId, message.UserScore, message.QuizScore);

        // TODO: dispatch email/push notification to user congratulating them on completing the mission.
        // Suggested payload: { UserId, MissionId, MissionName, QuizId, UserScore, QuizScore }
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
