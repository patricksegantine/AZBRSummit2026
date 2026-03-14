using UserAccount.Api.Infrastructure.Messaging;
using UserAccount.Api.Infrastructure.Persistence;

namespace UserAccount.Api.Features.Accounts.CreateAccount;

public class CreateAccountHandler(AppDbContext db, ServiceBusPublisher publisher)
{
    public async Task<UserAccountEntity> HandleAsync(CreateAccountRequest request, CancellationToken ct)
    {
        var entity = new UserAccountEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Cpf = request.Cpf ?? string.Empty,
            DataDictionary = request.DataDictionary,
            CreatedAt = DateTime.UtcNow
        };

        db.UserAccounts.Add(entity);
        await db.SaveChangesAsync(ct);

        var campaignProperties = ExtractCampaignProperties(request.DataDictionary);

        await publisher.PublishAsync("user-account-added-or-updated", new
        {
            entity.Id,
            entity.Name,
            entity.Email,
            entity.CreatedAt
        }, campaignProperties, ct);

        return entity;
    }

    private static Dictionary<string, object>? ExtractCampaignProperties(List<Dictionary<string, string>> dataDictionary)
    {
        var campaignId = dataDictionary
            .Select(d => d.GetValueOrDefault("CampaignId"))
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        var missionId = dataDictionary
            .Select(d => d.GetValueOrDefault("MissionId"))
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        var indicationToken = dataDictionary
            .Select(d => d.GetValueOrDefault("IndicationToken"))
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        return new Dictionary<string, object>
        {
            ["EventName"] = "user-account-created",
            ["MissionId"] = missionId,
            ["CampaignId"] = campaignId,
            ["IndicationToken"] = indicationToken
        };
    }
}
