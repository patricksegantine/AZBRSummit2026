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
            DataDictionary = request.DataDictionary,
            CreatedAt = DateTime.UtcNow
        };

        db.UserAccounts.Add(entity);
        await db.SaveChangesAsync(ct);

        var campaignProperties = ExtractCampaignProperties(request.DataDictionary);

        await publisher.PublishAsync("user-account-created", new
        {
            entity.Id,
            entity.Name,
            entity.Email,
            entity.CreatedAt
        }, campaignProperties, ct);

        return entity;
    }

    private static IDictionary<string, object>? ExtractCampaignProperties(List<Dictionary<string, string>> dataDictionary)
    {
        var campaignId = dataDictionary
            .Select(d => d.GetValueOrDefault("CampaignId"))
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        var indicationToken = dataDictionary
            .Select(d => d.GetValueOrDefault("IndicationToken"))
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        if (campaignId is null || indicationToken is null)
            return null;

        return new Dictionary<string, object>
        {
            ["CampaignId"] = campaignId,
            ["IndicationToken"] = indicationToken
        };
    }
}
