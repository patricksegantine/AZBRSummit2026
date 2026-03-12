namespace Mission.QuizCompleteAnalyzer.Worker.Messaging;

public record QuizAnsweredMessage(
    Guid QuizId,
    Guid UserId,
    int QuizScore,
    int UserScore,
    bool UserGotAward
);
