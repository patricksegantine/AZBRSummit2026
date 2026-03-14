using Microsoft.EntityFrameworkCore;
using UserAccount.Api.Infrastructure.Persistence;

namespace UserAccount.Api.Features.Accounts.GetAccount;

public class GetAccountsHandler(AppDbContext db)
{
    public async Task<GetAccountResponse?> HandleAsync(GetAccountsQuery query, CancellationToken ct)
    {
        var queryable = db.UserAccounts.AsQueryable();

        if (query.Id.HasValue)
            queryable = queryable.Where(x => x.Id == query.Id.Value);

        if (!string.IsNullOrWhiteSpace(query.Email))
            queryable = queryable.Where(x => x.Email == query.Email);

        if (!string.IsNullOrWhiteSpace(query.Cpf))
            queryable = queryable.Where(x => x.Cpf == query.Cpf);

        var entity = await queryable.FirstOrDefaultAsync(ct);

        if (entity is null)
            return null;

        return new GetAccountResponse(
            entity.Id,
            entity.Name,
            entity.Email,
            entity.PhoneNumber,
            entity.Cpf,
            entity.DataDictionary,
            entity.CreatedAt
        );
    }
}
