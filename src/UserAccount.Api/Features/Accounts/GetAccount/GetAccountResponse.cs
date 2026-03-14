namespace UserAccount.Api.Features.Accounts.GetAccount;

public record GetAccountResponse(
    Guid Id,
    string Name,
    string Email,
    string PhoneNumber,
    string Cpf,
    List<Dictionary<string, string>> DataDictionary,
    DateTime CreatedAt
);
