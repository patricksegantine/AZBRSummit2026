namespace UserAccount.Api.Features.Accounts.CreateAccount;

public record CreateAccountRequest(
    string Name,
    string Email,
    List<Dictionary<string, string>> DataDictionary
);
