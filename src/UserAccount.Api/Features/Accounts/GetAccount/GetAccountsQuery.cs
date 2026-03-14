namespace UserAccount.Api.Features.Accounts.GetAccount;

public record GetAccountsQuery(
    Guid? Id,
    string? Email,
    string? Cpf
);
