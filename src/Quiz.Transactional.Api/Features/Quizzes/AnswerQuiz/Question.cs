namespace Quiz.Transactional.Api.Features.Quizzes.AnswerQuiz;

public class Question
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public List<Alternative> Alternatives { get; init; } = [];
}
