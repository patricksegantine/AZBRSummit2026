namespace UserAccount.Api.Features.Accounts.CreateAccount;

public class UserAccountEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Dictionary<string, string>> DataDictionary { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
