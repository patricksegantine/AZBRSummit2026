namespace Mission.QuizCompleteAnalyzer.Worker.Domain;

public enum ChallengeType { Quiz, MGM, Training }

public enum MissionStatus { Pending, Active, Completed, Cancelled }

public class Mission
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid? MediaId { get; init; }
    public Guid? CampaignId { get; init; }
    public ChallengeType Type { get; init; }
    public MissionStatus Status { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
}
