using Mission.Domain;
using Mission.Infrastructure.Data;

namespace Mission.QuizCompleteAnalyzer.Worker.Handlers;

public record QuizCompletionCommand(
    Guid UserId,
    Guid MissionId,
    Guid QuizAnswerId,
    int UserScore,
    int QuizScore,
    bool UserGotAward
);

public sealed class QuizCompletionResult
{
    public bool IsSuccess { get; private init; }
    public string? FailureCode { get; private init; }
    public string? FailureReason { get; private init; }
    public UserMission? UserMission { get; private init; }

    public static QuizCompletionResult Ok(UserMission userMission) =>
        new() { IsSuccess = true, UserMission = userMission };

    public static QuizCompletionResult Fail(string code, string reason) =>
        new() { IsSuccess = false, FailureCode = code, FailureReason = reason };
}

public class QuizCompletionHandler(
    MissionStore missionStore,
    UserMissionStore userMissionStore)
{
    public QuizCompletionResult Handle(QuizCompletionCommand command)
    {
        var mission = missionStore.GetById(command.MissionId);
        if (mission is null)
            return QuizCompletionResult.Fail(
                "MissionNotFound",
                $"Missão não encontrada: '{command.MissionId}'.");

        if (mission.Type != MissionType.Quiz)
            return QuizCompletionResult.Fail(
                "InvalidMissionType",
                $"A missão '{mission.Id}' não é do tipo Quiz.");

        if (command.UserScore < command.QuizScore)
            return QuizCompletionResult.Fail(
                "MinimumScoreNotReached",
                $"Usuário {command.UserId} não atingiu a pontuação mínima no quiz {command.QuizAnswerId}. Pontuação: {command.UserScore}/{command.QuizScore}.");

        var userMission = new UserMission
        {
            Id = Guid.NewGuid(),

            // Associa a resposta do quiz à missão para rastreabilidade
            QuizAnswerId = command.QuizAnswerId,

            Status = UserMissionStatus.Completed,
            CompletedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };

        userMissionStore.Add(userMission);

        // TODO: dispatch email/push notification to the user congratulating them on completing the quiz mission.
        // Suggested payload: { UserId, MissionId, MissionName, QuizId, UserScore, QuizScore }
        // Channels: email via SendGrid/Communication Services, push via Azure Notification Hubs.

        return QuizCompletionResult.Ok(userMission);
    }
}
