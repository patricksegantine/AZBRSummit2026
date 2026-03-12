namespace Quiz.Transactional.Api.Features.Quizzes.AnswerQuiz;

public enum QuizStatus { Draft, Active, Closed }

public class Quiz
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid? MediaId { get; init; }
    public Guid? SegmentationId { get; init; }
    public List<Question> Questions { get; init; } = [];
    public bool HasAward { get; init; }
    public Guid? AwardGamificationId { get; init; }
    public int TotalWinners { get; init; }
    public bool HasMinimalPercentage { get; init; }
    public int MinimalPercentage { get; init; }
    public QuizStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
