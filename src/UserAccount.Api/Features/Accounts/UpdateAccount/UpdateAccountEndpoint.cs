namespace UserAccount.Api.Features.Accounts.UpdateAccount;

public static class UpdateAccountEndpoint
{
    public static IEndpointRouteBuilder MapUpdateAccount(this IEndpointRouteBuilder app)
    {
        app.MapPut("/accounts/{id:guid}", async (
            Guid id,
            UpdateAccountRequest request,
            UpdateAccountHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("UpdateAccount")
        .WithTags("Accounts");

        return app;
    }
}
