namespace UserAccount.Api.Features.Accounts.UpdateAccount;

public record UpdateAccountRequest(
    string Name,
    string Email,
    string PhoneNumber,
    List<Dictionary<string, string>> DataDictionary
);
