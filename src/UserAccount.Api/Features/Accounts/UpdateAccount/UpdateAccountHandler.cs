using UserAccount.Api.Features.Accounts.CreateAccount;
using UserAccount.Api.Infrastructure.Messaging;
using UserAccount.Api.Infrastructure.Persistence;

namespace UserAccount.Api.Features.Accounts.UpdateAccount;

public class UpdateAccountHandler(AppDbContext db, ServiceBusPublisher publisher)
{
    public async Task<UserAccountEntity?> HandleAsync(Guid id, UpdateAccountRequest request, CancellationToken ct)
    {
        var entity = await db.UserAccounts.FindAsync([id], ct);

        if (entity is null)
            return null;

        entity.Name = request.Name;
        entity.Email = request.Email;
        entity.PhoneNumber = request.PhoneNumber;
        entity.DataDictionary = request.DataDictionary;

        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(
            topicName: "user-account-added-or-updated",
            message: new
            {
                entity.Id,
                entity.Name,
                entity.Email,
                entity.CreatedAt
            },
            properties: new Dictionary<string, object>
            {
                ["EventName"] = "user-account-updated",
            },
            ct: ct);

        return entity;
    }
}
