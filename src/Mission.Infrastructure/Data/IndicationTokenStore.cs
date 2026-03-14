using Mission.Domain;

namespace Mission.Infrastructure.Data;

public class IndicationTokenStore
{
    private readonly List<IndicationToken> _tokens =
    [
        new IndicationToken
        {
            Id = SeedData.IndicationTokens.Token1Id,
            Token = "ALCE26",
            UserId = SeedData.IndicationTokens.OwnerUser1,
            MissionId = SeedData.Missions.Mission1Id,
            Status = IndicationTokenStatus.Active,
            CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc),
            ExpiresAt = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc)
        },
        new IndicationToken
        {
            Id = SeedData.IndicationTokens.Token2Id,
            Token = "BOBS42",
            UserId = SeedData.IndicationTokens.OwnerUser2,
            MissionId = SeedData.Missions.Mission2Id,
            Status = IndicationTokenStatus.Active,
            CreatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc),
            ExpiresAt = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc)
        },
        new IndicationToken
        {
            Id = SeedData.IndicationTokens.Token3Id,
            Token = "CARL99",
            UserId = SeedData.IndicationTokens.OwnerUser3,
            MissionId = SeedData.Missions.Mission1Id,
            Status = IndicationTokenStatus.Expired,
            CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            ExpiresAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc)
        },
        new IndicationToken
        {
            Id = SeedData.IndicationTokens.Token4Id,
            Token = "DAVE01",
            UserId = SeedData.IndicationTokens.OwnerUser4,
            MissionId = SeedData.Missions.Mission1Id,
            Status = IndicationTokenStatus.Revoked,
            CreatedAt = new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc),
            ExpiresAt = null
        }
    ];

    public void Add(IndicationToken token) =>
        _tokens.Add(token);

    public IndicationToken? GetByToken(string token) =>
        _tokens.FirstOrDefault(t => t.Token == token);

    public List<IndicationToken> GetByOwnerId(Guid ownerId) =>
        _tokens.Where(t => t.UserId == ownerId).ToList();
}
