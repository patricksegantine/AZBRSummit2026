using MissionEntity = Mission.MgmCompleteAnalyzer.Worker.Domain.Mission;
using Mission.MgmCompleteAnalyzer.Worker.Domain;

namespace Mission.MgmCompleteAnalyzer.Worker.Infrastructure.Data;

public class MissionStore
{
    private readonly Dictionary<Guid, MissionEntity> _missions = new()
    {
        {
            new Guid("55555555-0000-0000-0000-000000000001"),
            new MissionEntity
            {
                Id = new Guid("55555555-0000-0000-0000-000000000001"),
                Name = "Indique um Amigo - Summit 2026",
                Description = "Indique um amigo para se cadastrar no Azure Brasil Summit 2026 e complete esta missão MGM.",
                MediaId = new Guid("aaaaaaaa-0000-0000-0000-000000000010"),
                CampaignId = new Guid("cccccccc-0000-0000-0000-000000000001"),
                Type = ChallengeType.MGM,
                Status = MissionStatus.Active,
                CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        },
        {
            new Guid("55555555-0000-0000-0000-000000000002"),
            new MissionEntity
            {
                Id = new Guid("55555555-0000-0000-0000-000000000002"),
                Name = "Embaixador da Comunidade",
                Description = "Traga 3 novos participantes para a comunidade Azure Brasil e ganhe o badge de Embaixador.",
                MediaId = new Guid("aaaaaaaa-0000-0000-0000-000000000011"),
                CampaignId = new Guid("cccccccc-0000-0000-0000-000000000002"),
                Type = ChallengeType.MGM,
                Status = MissionStatus.Active,
                CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        }
    };

    public MissionEntity? GetByCampaignId(Guid campaignId) =>
        _missions.Values.FirstOrDefault(m => m.CampaignId == campaignId && m.Type == ChallengeType.MGM);

    public void Complete(Guid id)
    {
        if (!_missions.TryGetValue(id, out var mission))
            return;

        mission.Status = MissionStatus.Completed;
        mission.UpdatedAt = DateTime.UtcNow;
    }
}
