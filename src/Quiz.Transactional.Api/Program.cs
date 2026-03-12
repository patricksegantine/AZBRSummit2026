using Azure.Messaging.ServiceBus;
using Quiz.Transactional.Api.Features.Quizzes.AnswerQuiz;
using Quiz.Transactional.Api.Infrastructure.Data;
using Quiz.Transactional.Api.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(_ =>
    new ServiceBusClient(builder.Configuration.GetConnectionString("AzureServiceBus")));

builder.Services.AddSingleton<QuizStore>();
builder.Services.AddScoped<ServiceBusPublisher>();
builder.Services.AddScoped<AnswerQuizHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapAnswerQuiz();

app.Run();
