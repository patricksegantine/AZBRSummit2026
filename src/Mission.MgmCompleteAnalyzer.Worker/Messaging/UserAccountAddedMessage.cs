namespace Mission.MgmCompleteAnalyzer.Worker.Messaging;

public record UserAccountAddedMessage(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt
);
