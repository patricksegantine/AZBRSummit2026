using Azure.Messaging.ServiceBus;
using Mission.QuizCompleteAnalyzer.Worker.Handlers;
using Mission.QuizCompleteAnalyzer.Worker.Messaging;
using System.Text.Json;

namespace Mission.QuizCompleteAnalyzer.Worker;

public class Worker(
    ServiceBusProcessor processor,
    QuizCompletionHandler completionHandler,
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

        // MissionId is guaranteed by the Service Bus subscription filter,
        // but we validate defensively in case the message arrives via another path.
        if (!args.Message.ApplicationProperties.TryGetValue("MissionId", out var missionIdRaw)
            || !Guid.TryParse(missionIdRaw?.ToString(), out var missionId))
        {
            logger.LogWarning(
                "Message is missing a valid MissionId property. QuizId: {QuizId} | UserId: {UserId}",
                message.QuizId, message.UserId);
            await args.DeadLetterMessageAsync(args.Message, "MissingMissionId", "ApplicationProperty 'MissionId' is absent or invalid.", args.CancellationToken);
            return;
        }

        var command = new QuizCompletionCommand(
            UserId: message.UserId,
            MissionId: missionId,
            QuizAnswerId: message.QuizId,
            UserScore: message.UserScore,
            QuizScore: message.QuizScore,
            UserGotAward: message.UserGotAward
        );

        var result = completionHandler.Handle(command);

        if (!result.IsSuccess)
        {
            logger.LogWarning(
                "Quiz completion failed. UserId: {UserId} | MissionId: {MissionId} | QuizId: {QuizId} | Reason: {Reason}",
                message.UserId, missionId, message.QuizId, result.FailureReason);
            await args.DeadLetterMessageAsync(args.Message, result.FailureCode, result.FailureReason, args.CancellationToken);
            return;
        }

        logger.LogInformation(
            "UserMission {UserMissionId} created: Quiz mission {MissionId} completed by user {UserId} | QuizId: {QuizId} | UserScore: {UserScore}/{QuizScore}",
            result.UserMission!.Id, missionId, message.UserId, message.QuizId, message.UserScore, message.QuizScore);

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
