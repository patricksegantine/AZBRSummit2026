using Mission.QuizCompleteAnalyzer.Worker.Domain;

namespace Mission.QuizCompleteAnalyzer.Worker.Infrastructure.Data;

public class MissionStore
{
    private readonly Dictionary<Guid, Domain.Mission> _missions = new()
    {
        {
            new Guid("44444444-0000-0000-0000-000000000001"),
            new Domain.Mission
            {
                Id = new Guid("44444444-0000-0000-0000-000000000001"),
                Name = "Azure Fundamentals Quiz Mission",
                Description = "Complete the Azure Fundamentals quiz with a minimum score of 70% to unlock your badge.",
                MediaId = new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                CampaignId = new Guid("cccccccc-0000-0000-0000-000000000001"),
                Type = ChallengeType.Quiz,
                Status = MissionStatus.Active,
                CreatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc)
            }
        },
        {
            new Guid("44444444-0000-0000-0000-000000000002"),
            new Domain.Mission
            {
                Id = new Guid("44444444-0000-0000-0000-000000000002"),
                Name = "Cloud Architecture Patterns Mission",
                Description = "Master cloud architecture by completing the patterns quiz with at least 60% correct answers.",
                MediaId = new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                CampaignId = new Guid("cccccccc-0000-0000-0000-000000000002"),
                Type = ChallengeType.Quiz,
                Status = MissionStatus.Active,
                CreatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc)
            }
        },
        {
            new Guid("44444444-0000-0000-0000-000000000003"),
            new Domain.Mission
            {
                Id = new Guid("44444444-0000-0000-0000-000000000003"),
                Name = "Refer a Friend Mission",
                Description = "Invite 3 colleagues to join the Azure Brasil Summit 2026 experience.",
                MediaId = null,
                CampaignId = new Guid("cccccccc-0000-0000-0000-000000000001"),
                Type = ChallengeType.MGM,
                Status = MissionStatus.Active,
                CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        }
    };

    public Domain.Mission? GetById(Guid id) =>
        _missions.GetValueOrDefault(id);

    public void Complete(Guid id)
    {
        if (!_missions.TryGetValue(id, out var mission))
            return;

        mission.Status = MissionStatus.Completed;
        mission.UpdatedAt = DateTime.UtcNow;
    }
}
