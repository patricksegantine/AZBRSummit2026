namespace Quiz.Transactional.Api.Features.Quizzes.AnswerQuiz;

public class Alternative
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Score { get; init; }
    public int Level { get; init; }
    public bool IsRightAnswer { get; init; }
}
