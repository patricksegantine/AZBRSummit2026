namespace UserAccount.Api.Features.Accounts.CreateAccount;

public static class CreateAccountEndpoint
{
    public static IEndpointRouteBuilder MapCreateAccount(this IEndpointRouteBuilder app)
    {
        app.MapPost("/accounts", async (
            CreateAccountRequest request,
            CreateAccountHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(request, ct);
            return Results.Created($"/accounts/{result.Id}", result);
        })
        .WithName("CreateAccount")
        .WithTags("Accounts");

        return app;
    }
}
