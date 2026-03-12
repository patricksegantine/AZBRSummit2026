namespace Quiz.Transactional.Api.Features.Quizzes.AnswerQuiz;

public record AnswerQuizRequest(
    Guid UserId,
    List<QuestionAnswer> Answers,
    List<Dictionary<string, string>> DataDictionary
);

public record QuestionAnswer(Guid QuestionId, Guid AlternativeId);
