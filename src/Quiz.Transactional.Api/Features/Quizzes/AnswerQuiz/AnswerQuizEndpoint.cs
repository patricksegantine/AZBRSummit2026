namespace Quiz.Transactional.Api.Features.Quizzes.AnswerQuiz;

public static class AnswerQuizEndpoint
{
    public static IEndpointRouteBuilder MapAnswerQuiz(this IEndpointRouteBuilder app)
    {
        app.MapPost("/quizzes/{id:guid}/answers", async (
            Guid id,
            AnswerQuizRequest request,
            AnswerQuizHandler handler,
            CancellationToken ct) =>
        {
            return await handler.HandleAsync(id, request, ct);
        })
        .WithName("AnswerQuiz")
        .WithTags("Quizzes");

        return app;
    }
}
