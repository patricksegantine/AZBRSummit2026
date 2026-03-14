using Mission.Domain;

namespace Mission.Infrastructure.Data;

public class UserMissionStore
{
    private readonly List<UserMission> _userMissions =
    [
        new UserMission
        {
            Id = SeedData.UserMissions.UserMission1Id,
            ReferredUserId = SeedData.UserMissions.ReferredUser1,
            IndicationTokenId = SeedData.IndicationTokens.Token1Id,
            Status = UserMissionStatus.Completed,
            CreatedAt = new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc),
            CompletedAt = new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc)
        },
        new UserMission
        {
            Id = SeedData.UserMissions.UserMission2Id,
            ReferredUserId = SeedData.UserMissions.ReferredUser2,
            IndicationTokenId = SeedData.IndicationTokens.Token2Id,
            Status = UserMissionStatus.Completed,
            CreatedAt = new DateTime(2026, 2, 10, 14, 30, 0, DateTimeKind.Utc),
            CompletedAt = new DateTime(2026, 2, 10, 14, 30, 0, DateTimeKind.Utc)
        },
        new UserMission
        {
            Id = SeedData.UserMissions.UserMission3Id,
            ReferredUserId = SeedData.UserMissions.ReferredUser3,
            IndicationTokenId = SeedData.IndicationTokens.Token1Id,
            Status = UserMissionStatus.Pending,
            CreatedAt = new DateTime(2026, 3, 1, 9, 0, 0, DateTimeKind.Utc),
            CompletedAt = null
        },

        // Quiz missions
        new UserMission
        {
            Id = SeedData.UserMissions.QuizUserMission1Id,
            ReferredUserId = SeedData.UserMissions.QuizUser1,
            QuizAnswerId = SeedData.UserMissions.QuizAnswer1Id,
            Status = UserMissionStatus.Completed,
            CreatedAt = new DateTime(2026, 2, 15, 9, 0, 0, DateTimeKind.Utc),
            CompletedAt = new DateTime(2026, 2, 15, 9, 0, 0, DateTimeKind.Utc)
        },
        new UserMission
        {
            Id = SeedData.UserMissions.QuizUserMission2Id,
            ReferredUserId = SeedData.UserMissions.QuizUser2,
            QuizAnswerId = SeedData.UserMissions.QuizAnswer2Id,
            Status = UserMissionStatus.Completed,
            CreatedAt = new DateTime(2026, 2, 20, 14, 0, 0, DateTimeKind.Utc),
            CompletedAt = new DateTime(2026, 2, 20, 14, 0, 0, DateTimeKind.Utc)
        },
        new UserMission
        {
            Id = SeedData.UserMissions.QuizUserMission3Id,
            ReferredUserId = SeedData.UserMissions.QuizUser3,
            QuizAnswerId = SeedData.UserMissions.QuizAnswer3Id,
            Status = UserMissionStatus.Pending,
            CreatedAt = new DateTime(2026, 3, 5, 11, 30, 0, DateTimeKind.Utc),
            CompletedAt = null
        }
    ];

    public void Add(UserMission userMission) =>
        _userMissions.Add(userMission);

    public UserMission? GetByUserAndToken(Guid userId, Guid indicationTokenId) =>
        _userMissions.FirstOrDefault(um => um.ReferredUserId == userId && um.IndicationTokenId == indicationTokenId);

    public List<UserMission> GetByReferredUserId(Guid userId) =>
        _userMissions.Where(um => um.ReferredUserId == userId).ToList();

    public List<UserMission> GetByIndicationTokenId(Guid indicationTokenId) =>
        _userMissions.Where(um => um.IndicationTokenId == indicationTokenId).ToList();
}
