namespace UserAccount.Api.Features.Accounts.GetAccount;

public static class GetAccountsEndpoint
{
    public static IEndpointRouteBuilder MapGetAccounts(this IEndpointRouteBuilder app)
    {
        app.MapGet("/accounts", async (
            Guid? id,
            string? email,
            string? cpf,
            GetAccountsHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetAccountsQuery(id, email, cpf), ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetAccount")
        .WithTags("Accounts");

        return app;
    }
}
