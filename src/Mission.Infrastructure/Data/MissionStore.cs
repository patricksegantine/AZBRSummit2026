using Mission.Domain;
using MissionEntity = Mission.Domain.Mission;

namespace Mission.Infrastructure.Data;

public class MissionStore
{
    private readonly Dictionary<Guid, MissionEntity> _missions = new()
    {
        {
            SeedData.Missions.Mission1Id,
            new MissionEntity
            {
                Id = SeedData.Missions.Mission1Id,
                Name = "Indique um Amigo - Summit 2026",
                Description = "Indique um amigo para se cadastrar no Azure Brasil Summit 2026 e complete esta missão MGM.",
                MediaId = SeedData.Missions.Mission1MediaId,
                CampaignId = SeedData.Missions.Mission1CampaignId,
                Type = MissionType.MGM,
                Status = MissionStatus.Active,
                CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        },
        {
            SeedData.Missions.Mission2Id,
            new MissionEntity
            {
                Id = SeedData.Missions.Mission2Id,
                Name = "Embaixador da Comunidade",
                Description = "Traga 3 novos participantes para a comunidade Azure Brasil e ganhe o badge de Embaixador.",
                MediaId = SeedData.Missions.Mission2MediaId,
                CampaignId = SeedData.Missions.Mission2CampaignId,
                Type = MissionType.MGM,
                Status = MissionStatus.Active,
                CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        },
        {
            SeedData.Missions.QuizMission1Id,
            new MissionEntity
            {
                Id = SeedData.Missions.QuizMission1Id,
                Name = "Azure Fundamentals Quiz Mission",
                Description = "Complete the Azure Fundamentals quiz with a minimum score of 70% to unlock your badge.",
                MediaId = SeedData.Missions.QuizMission1MediaId,
                CampaignId = SeedData.Missions.QuizMission1CampaignId,
                Type = MissionType.Quiz,
                Status = MissionStatus.Active,
                CreatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc)
            }
        },
        {
            SeedData.Missions.QuizMission2Id,
            new MissionEntity
            {
                Id = SeedData.Missions.QuizMission2Id,
                Name = "Cloud Architecture Patterns Mission",
                Description = "Master cloud architecture by completing the patterns quiz with at least 60% correct answers.",
                MediaId = SeedData.Missions.QuizMission2MediaId,
                CampaignId = SeedData.Missions.QuizMission2CampaignId,
                Type = MissionType.Quiz,
                Status = MissionStatus.Active,
                CreatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc)
            }
        },
        {
            SeedData.Missions.QuizMission3Id,
            new MissionEntity
            {
                Id = SeedData.Missions.QuizMission3Id,
                Name = "Refer a Friend Mission",
                Description = "Invite 3 colleagues to join the Azure Brasil Summit 2026 experience.",
                MediaId = null,
                CampaignId = SeedData.Missions.QuizMission1CampaignId,
                Type = MissionType.MGM,
                Status = MissionStatus.Active,
                CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        }
    };

    public MissionEntity? GetById(Guid id) =>
        _missions.TryGetValue(id, out var mission) ? mission : null;

    public void Complete(Guid id)
    {
        if (!_missions.TryGetValue(id, out var mission))
            return;

        mission.Status = MissionStatus.Completed;
        mission.UpdatedAt = DateTime.UtcNow;
    }
}
