using QuizModel = Quiz.Transactional.Api.Features.Quizzes.AnswerQuiz.Quiz;
using Quiz.Transactional.Api.Features.Quizzes.AnswerQuiz;

namespace Quiz.Transactional.Api.Infrastructure.Data;

public class QuizStore
{
    private static readonly List<QuizModel> _quizzes =
    [
        new()
        {
            Id = new Guid("11111111-0000-0000-0000-000000000001"),
            Name = "Azure Fundamentals Challenge",
            Description = "Test your knowledge on core Azure services and concepts.",
            MediaId = new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
            SegmentationId = new Guid("bbbbbbbb-0000-0000-0000-000000000001"),
            HasAward = true,
            AwardGamificationId = new Guid("cccccccc-0000-0000-0000-000000000001"),
            TotalWinners = 100,
            HasMinimalPercentage = true,
            MinimalPercentage = 70,
            Status = QuizStatus.Active,
            CreatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            Questions =
            [
                new()
                {
                    Id = new Guid("22222222-0000-0000-0000-000000000001"),
                    Title = "Which Azure service is best suited for hosting containerized applications without managing infrastructure?",
                    Alternatives =
                    [
                        new() { Id = new Guid("33333333-0000-0000-0001-000000000001"), Title = "Azure Virtual Machines", Score = 0, Level = 1, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0002-000000000001"), Title = "Azure Container Apps", Score = 10, Level = 1, IsRightAnswer = true },
                        new() { Id = new Guid("33333333-0000-0000-0003-000000000001"), Title = "Azure Blob Storage", Score = 0, Level = 1, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0004-000000000001"), Title = "Azure API Management", Score = 0, Level = 1, IsRightAnswer = false }
                    ]
                },
                new()
                {
                    Id = new Guid("22222222-0000-0000-0000-000000000002"),
                    Title = "What is the primary purpose of Azure Service Bus?",
                    Alternatives =
                    [
                        new() { Id = new Guid("33333333-0000-0000-0001-000000000002"), Title = "Store large binary files", Score = 0, Level = 2, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0002-000000000002"), Title = "Host static websites", Score = 0, Level = 2, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0003-000000000002"), Title = "Reliable asynchronous messaging between services", Score = 10, Level = 2, IsRightAnswer = true },
                        new() { Id = new Guid("33333333-0000-0000-0004-000000000002"), Title = "Run scheduled background jobs", Score = 0, Level = 2, IsRightAnswer = false }
                    ]
                },
                new()
                {
                    Id = new Guid("22222222-0000-0000-0000-000000000003"),
                    Title = "Which resource provides a globally distributed, multi-model database service on Azure?",
                    Alternatives =
                    [
                        new() { Id = new Guid("33333333-0000-0000-0001-000000000003"), Title = "Azure SQL Database", Score = 0, Level = 3, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0002-000000000003"), Title = "Azure Table Storage", Score = 0, Level = 3, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0003-000000000003"), Title = "Azure Cache for Redis", Score = 0, Level = 3, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0004-000000000003"), Title = "Azure Cosmos DB", Score = 10, Level = 3, IsRightAnswer = true }
                    ]
                }
            ]
        },
        new()
        {
            Id = new Guid("11111111-0000-0000-0000-000000000002"),
            Name = "Cloud Architecture Patterns",
            Description = "Evaluate your understanding of distributed systems and cloud design patterns.",
            MediaId = new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
            SegmentationId = new Guid("bbbbbbbb-0000-0000-0000-000000000002"),
            HasAward = true,
            AwardGamificationId = new Guid("cccccccc-0000-0000-0000-000000000002"),
            TotalWinners = 50,
            HasMinimalPercentage = true,
            MinimalPercentage = 60,
            Status = QuizStatus.Active,
            CreatedAt = new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc),
            Questions =
            [
                new()
                {
                    Id = new Guid("22222222-0000-0000-0000-000000000004"),
                    Title = "Which pattern decouples event producers from consumers using a pub/sub model?",
                    Alternatives =
                    [
                        new() { Id = new Guid("33333333-0000-0000-0001-000000000004"), Title = "Strangler Fig", Score = 0, Level = 1, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0002-000000000004"), Title = "Event-Driven Architecture", Score = 10, Level = 1, IsRightAnswer = true },
                        new() { Id = new Guid("33333333-0000-0000-0003-000000000004"), Title = "CQRS", Score = 0, Level = 1, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0004-000000000004"), Title = "Saga Pattern", Score = 0, Level = 1, IsRightAnswer = false }
                    ]
                },
                new()
                {
                    Id = new Guid("22222222-0000-0000-0000-000000000005"),
                    Title = "What does CQRS stand for?",
                    Alternatives =
                    [
                        new() { Id = new Guid("33333333-0000-0000-0001-000000000005"), Title = "Command Queue Retry Strategy", Score = 0, Level = 2, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0002-000000000005"), Title = "Consolidated Query and Resource Sync", Score = 0, Level = 2, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0003-000000000005"), Title = "Command and Query Responsibility Segregation", Score = 10, Level = 2, IsRightAnswer = true },
                        new() { Id = new Guid("33333333-0000-0000-0004-000000000005"), Title = "Centralized Queue for Request Scheduling", Score = 0, Level = 2, IsRightAnswer = false }
                    ]
                },
                new()
                {
                    Id = new Guid("22222222-0000-0000-0000-000000000006"),
                    Title = "Which pattern is used to manage long-running distributed transactions across multiple services?",
                    Alternatives =
                    [
                        new() { Id = new Guid("33333333-0000-0000-0001-000000000006"), Title = "Circuit Breaker", Score = 0, Level = 3, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0002-000000000006"), Title = "Bulkhead", Score = 0, Level = 3, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0003-000000000006"), Title = "Retry Pattern", Score = 0, Level = 3, IsRightAnswer = false },
                        new() { Id = new Guid("33333333-0000-0000-0004-000000000006"), Title = "Saga Pattern", Score = 10, Level = 3, IsRightAnswer = true }
                    ]
                }
            ]
        }
    ];

    public QuizModel? GetById(Guid id) =>
        _quizzes.FirstOrDefault(q => q.Id == id);
}
