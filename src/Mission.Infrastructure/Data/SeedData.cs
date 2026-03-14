namespace Mission.Infrastructure.Data;

/// <summary>
/// Centralized seed IDs shared across MissionStore, IndicationTokenStore and UserMissionStore.
/// </summary>
public static class SeedData
{
    public static class Missions
    {
        // MGM missions
        public static readonly Guid Mission1Id = new("55555555-0000-0000-0000-000000000001");
        public static readonly Guid Mission2Id = new("55555555-0000-0000-0000-000000000002");

        public static readonly Guid Mission1CampaignId = new("cccccccc-0000-0000-0000-000000000001");
        public static readonly Guid Mission2CampaignId = new("cccccccc-0000-0000-0000-000000000002");

        public static readonly Guid Mission1MediaId = new("aaaaaaaa-0000-0000-0000-000000000010");
        public static readonly Guid Mission2MediaId = new("aaaaaaaa-0000-0000-0000-000000000011");

        // Quiz missions
        public static readonly Guid QuizMission1Id = new("44444444-0000-0000-0000-000000000001");
        public static readonly Guid QuizMission2Id = new("44444444-0000-0000-0000-000000000002");
        public static readonly Guid QuizMission3Id = new("44444444-0000-0000-0000-000000000003");

        public static readonly Guid QuizMission1CampaignId = new("cccccccc-0000-0000-0000-000000000001");
        public static readonly Guid QuizMission2CampaignId = new("cccccccc-0000-0000-0000-000000000002");

        public static readonly Guid QuizMission1MediaId = new("aaaaaaaa-0000-0000-0000-000000000001");
        public static readonly Guid QuizMission2MediaId = new("aaaaaaaa-0000-0000-0000-000000000002");
    }

    public static class IndicationTokens
    {
        // Owner user IDs (users who generated the tokens)
        public static readonly Guid OwnerUser1 = new("dddddddd-0000-0000-0000-000000000001"); // ALCE26
        public static readonly Guid OwnerUser2 = new("dddddddd-0000-0000-0000-000000000002"); // BOBS42
        public static readonly Guid OwnerUser3 = new("dddddddd-0000-0000-0000-000000000003"); // CARL99
        public static readonly Guid OwnerUser4 = new("dddddddd-0000-0000-0000-000000000004"); // DAVE01

        // Token IDs
        public static readonly Guid Token1Id = new("eeeeeeee-0000-0000-0000-000000000001"); // ALCE26 → Mission1
        public static readonly Guid Token2Id = new("eeeeeeee-0000-0000-0000-000000000002"); // BOBS42 → Mission2
        public static readonly Guid Token3Id = new("eeeeeeee-0000-0000-0000-000000000003"); // CARL99 → Mission1 (Expired)
        public static readonly Guid Token4Id = new("eeeeeeee-0000-0000-0000-000000000004"); // DAVE01 → Mission1 (Revoked)
    }

    public static class UserMissions
    {
        // Referred user IDs (users who were indicated and registered) — MGM missions
        public static readonly Guid ReferredUser1 = new("ffffffff-0000-0000-0000-000000000001"); // via ALCE26
        public static readonly Guid ReferredUser2 = new("ffffffff-0000-0000-0000-000000000002"); // via BOBS42
        public static readonly Guid ReferredUser3 = new("ffffffff-0000-0000-0000-000000000003"); // via ALCE26 (Pending)

        // MGM UserMission IDs
        public static readonly Guid UserMission1Id = new("cccccccc-1111-0000-0000-000000000001");
        public static readonly Guid UserMission2Id = new("cccccccc-1111-0000-0000-000000000002");
        public static readonly Guid UserMission3Id = new("cccccccc-1111-0000-0000-000000000003");

        // Quiz user IDs
        public static readonly Guid QuizUser1 = new("bbbbbbbb-0000-0000-0000-000000000001");
        public static readonly Guid QuizUser2 = new("bbbbbbbb-0000-0000-0000-000000000002");
        public static readonly Guid QuizUser3 = new("bbbbbbbb-0000-0000-0000-000000000003");

        // Quiz answer IDs (QuizId from the message)
        public static readonly Guid QuizAnswer1Id = new("11111111-aaaa-0000-0000-000000000001"); // Azure Fundamentals → QuizMission1
        public static readonly Guid QuizAnswer2Id = new("11111111-aaaa-0000-0000-000000000002"); // Cloud Architecture → QuizMission2
        public static readonly Guid QuizAnswer3Id = new("11111111-aaaa-0000-0000-000000000003"); // Azure Fundamentals → QuizMission1 (Pending)

        // Quiz UserMission IDs
        public static readonly Guid QuizUserMission1Id = new("cccccccc-2222-0000-0000-000000000001");
        public static readonly Guid QuizUserMission2Id = new("cccccccc-2222-0000-0000-000000000002");
        public static readonly Guid QuizUserMission3Id = new("cccccccc-2222-0000-0000-000000000003");
    }
}
