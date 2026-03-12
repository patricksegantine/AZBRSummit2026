# Arquitetura do Sistema

## Visão Geral dos Componentes

```mermaid
graph TB
    Client(["👤 Client / Mobile App"])

    subgraph APIs["APIs — ASP.NET Core Minimal API"]
        UA["**UserAccount.Api**\nPOST /accounts"]
        QT["**Quiz.Transactional.Api**\nPOST /quizzes/{id}/answers"]
    end

    subgraph Storage["Storage"]
        PG[("PostgreSQL\nuser_account_db")]
        QS[("In-Memory\nQuizStore")]
    end

    subgraph ASB["Azure Service Bus"]
        direction TB
        T1["📨 Topic\nuser-account-created"]
        T2["📨 Topic\nquiz-answered"]

        subgraph Subs1["Subscriptions — user-account-created"]
            S1["🔍 mgm-filter\nCampaignId AND IndicationToken"]
        end

        subgraph Subs2["Subscriptions — quiz-answered"]
            S2["🔍 mission-quiz-filter\nMissionId EXISTS"]
            S3["🔍 quiz-benefit-filter"]
        end

        Q1["📥 Queue\nmgm-user-account-added-analyzer"]
        Q2["📥 Queue\nmission-quiz-answer-analyzer"]
        Q3["📥 Queue\nquiz-benefit-queue"]
    end

    subgraph Workers["Workers — .NET BackgroundService"]
        W1["**Mission.MgmCompleteAnalyzer**\n.Worker"]
        W2["**Mission.QuizCompleteAnalyzer**\n.Worker"]
        W3["**Quiz.Benefit.Worker**"]
    end

    Client -->|"CreateAccount\n{Name, Email, DataDictionary}"| UA
    Client -->|"AnswerQuiz\n{UserId, Answers[], DataDictionary}"| QT

    UA -->|"Persist UserAccount"| PG
    UA -->|"Publish event\n+ props: CampaignId, IndicationToken"| T1

    QT -->|"Lookup quiz"| QS
    QT -->|"Publish event\n+ prop: MissionId"| T2

    T1 --> S1
    S1 -->|"Forward"| Q1

    T2 --> S2
    T2 --> S3
    S2 -->|"Forward"| Q2
    S3 -->|"Forward"| Q3

    Q1 -->|"Consume"| W1
    Q2 -->|"Consume"| W2
    Q3 -->|"Consume"| W3
```

## Responsabilidades por Camada

| Componente | Tipo | Responsabilidade Principal |
|---|---|---|
| `UserAccount.Api` | Minimal API | Cadastro de usuário + publicação de evento de conta criada |
| `Quiz.Transactional.Api` | Minimal API | Submissão de respostas + cálculo de score + publicação de resultado |
| `Mission.MgmCompleteAnalyzer.Worker` | Background Worker | Detecta indicações MGM e conclui missões Member-Get-Member |
| `Mission.QuizCompleteAnalyzer.Worker` | Background Worker | Detecta conclusão de quizzes com score mínimo e conclui missões Quiz |
| `Quiz.Benefit.Worker` | Background Worker | Processa benefícios (gamificação) para quizzes completados |
| Azure Service Bus | Mensageria | Desacoplamento assíncrono + roteamento por filtros de propriedades |
| PostgreSQL | Banco de dados | Persistência das contas de usuário |
