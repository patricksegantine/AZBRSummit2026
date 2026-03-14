using Mission.Domain;
using Mission.Infrastructure.Data;

namespace Mission.MgmCompleteAnalyzer.Worker.Handlers;

public record MgmCompletionCommand(
    Guid UserId,
    string UserEmail,
    Guid MissionId,
    string IndicationToken
);

public sealed class MgmCompletionResult
{
    public bool IsSuccess { get; private init; }
    public string? FailureCode { get; private init; }
    public string? FailureReason { get; private init; }
    public UserMission? UserMission { get; private init; }

    public static MgmCompletionResult Ok(UserMission userMission) =>
        new() { IsSuccess = true, UserMission = userMission };

    public static MgmCompletionResult Fail(string code, string reason) =>
        new() { IsSuccess = false, FailureCode = code, FailureReason = reason };
}

public class MgmCompletionHandler(
    MissionStore missionStore,
    IndicationTokenStore indicationTokenStore,
    UserMissionStore userMissionStore)
{
    public MgmCompletionResult Handle(MgmCompletionCommand command)
    {
        var mission = missionStore.GetById(command.MissionId);
        if (mission is null)
            return MgmCompletionResult.Fail(
                "MissionNotFound",
                $"Missão não encontrada: '{command.MissionId}'.");

        if (mission.Type != MissionType.MGM)
            return MgmCompletionResult.Fail(
                "InvalidMissionType",
                $"A missão '{mission.Id}' não é do tipo MGM.");

        var indicationToken = indicationTokenStore.GetByToken(command.IndicationToken);
        var tokenValidation = ValidateIndicationToken(indicationToken);
        if (!tokenValidation.IsSuccess)
            return tokenValidation;

        var userMission = new UserMission
        {
            Id = Guid.NewGuid(),

            // O usuário que foi indicado
            ReferredUserId = command.UserId,

            // O token de indicação utilizado para completar a missão
            IndicationTokenId = indicationToken!.Id,

            // Pode haver regras adicionais para determinar o status,
            // como efetuar o pagamento de uma assinatura ou
            // validar o email do usuário indicado.
            Status = UserMissionStatus.Completed,
            CompletedAt = DateTime.UtcNow,
            
            CreatedAt = DateTime.UtcNow,
        };

        userMissionStore.Add(userMission);

        // TODO: dispatch email/push notification to the referrer identified by IndicationToken
        //       congratulating them on completing the MGM mission.
        // Suggested payload: { IndicationToken, MissionId, MissionName, ReferredAccountId, ReferredEmail }
        // Channels: email via SendGrid/Communication Services, push via Azure Notification Hubs.

        return MgmCompletionResult.Ok(userMission);
    }

    private static MgmCompletionResult ValidateIndicationToken(IndicationToken? indicationToken)
    {
        if (indicationToken is null)
            return MgmCompletionResult.Fail("InvalidIndicationToken", "Token não encontrado.");

        if (indicationToken.Status == IndicationTokenStatus.Revoked)
            return MgmCompletionResult.Fail("InvalidIndicationToken", "Token foi revogado.");

        if (indicationToken.Status == IndicationTokenStatus.Expired)
            return MgmCompletionResult.Fail("InvalidIndicationToken", "Token está expirado.");

        if (indicationToken.ExpiresAt.HasValue && indicationToken.ExpiresAt < DateTime.UtcNow)
        {
            indicationToken.Status = IndicationTokenStatus.Expired;
            return MgmCompletionResult.Fail("InvalidIndicationToken", "Token está expirado.");
        }

        return MgmCompletionResult.Ok(null!);
    }
}
