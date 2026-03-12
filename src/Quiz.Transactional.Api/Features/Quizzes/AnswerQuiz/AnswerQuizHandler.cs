using Quiz.Transactional.Api.Infrastructure.Data;
using Quiz.Transactional.Api.Infrastructure.Messaging;

namespace Quiz.Transactional.Api.Features.Quizzes.AnswerQuiz;

public class AnswerQuizHandler(QuizStore store, ServiceBusPublisher publisher)
{
    public async Task<IResult> HandleAsync(Guid quizId, AnswerQuizRequest request, CancellationToken ct)
    {
        var quiz = store.GetById(quizId);
        if (quiz is null)
            return Results.NotFound($"Quiz '{quizId}' not found.");

        if (quiz.Status != QuizStatus.Active)
            return Results.UnprocessableEntity($"Quiz '{quizId}' is not active.");

        var quizScore = quiz.Questions
            .SelectMany(q => q.Alternatives)
            .Where(a => a.IsRightAnswer)
            .Sum(a => a.Score);

        var userScore = request.Answers
            .Select(answer =>
            {
                var question = quiz.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                var alternative = question?.Alternatives.FirstOrDefault(a => a.Id == answer.AlternativeId);
                return alternative?.IsRightAnswer == true ? alternative.Score : 0;
            })
            .Sum();

        var userGotAward = quiz.HasAward && (!quiz.HasMinimalPercentage
            || quizScore > 0 && (double)userScore / quizScore * 100 >= quiz.MinimalPercentage);

        var missionId = ExtractMissionId(request.DataDictionary);

        var payload = new
        {
            QuizId = quizId,
            request.UserId,
            QuizScore = quizScore,
            UserScore = userScore,
            UserGotAward = userGotAward
        };

        IDictionary<string, object>? properties = missionId is not null
            ? new Dictionary<string, object> { ["MissionId"] = missionId }
            : null;

        await publisher.PublishAsync("quiz-answered", payload, properties, ct);

        return Results.Ok(payload);
    }

    private static string? ExtractMissionId(List<Dictionary<string, string>> dataDictionary) =>
        dataDictionary
            .Select(d => d.GetValueOrDefault("MissionId"))
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
}
