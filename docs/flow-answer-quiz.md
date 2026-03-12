# Fluxo — Resposta de Quiz

## Sequência Completa

```mermaid
sequenceDiagram
    actor Client as 👤 Client
    participant API as Quiz.Transactional.Api
    participant QS as QuizStore (In-Memory)
    participant SB as Azure Service Bus
    participant MQW as Mission.QuizCompleteAnalyzer.Worker
    participant QBW as Quiz.Benefit.Worker

    Client->>+API: POST /quizzes/{id}/answers\n{UserId, Answers[], DataDictionary}

    API->>+QS: GetById(quizId)
    QS-->>-API: Quiz | null

    alt Quiz não encontrado
        API-->>Client: 404 Not Found
    else Quiz não está Active
        API-->>Client: 422 Unprocessable Entity
    else Quiz encontrado e Active
        API->>API: Calcula quizScore\n(soma Score das alternativas corretas)
        API->>API: Calcula userScore\n(soma Score das respostas corretas do usuário)
        API->>API: Determina userGotAward\n(HasAward AND score% >= MinimalPercentage)
        API->>API: ExtractMissionId(DataDictionary)

        alt MissionId presente no DataDictionary
            API->>+SB: Publish → topic: quiz-answered\nBody: {QuizId, UserId, QuizScore, UserScore, UserGotAward}\nProps: {MissionId}
            SB-->>-API: Accepted

            Note over SB: mission-quiz-filter: MissionId IS NOT NULL\nquiz-benefit-filter: 1=1

            par Missão Quiz
                SB->>+MQW: Forward → queue: mission-quiz-answer-analyzer
                MQW->>MQW: Extrai MissionId das ApplicationProperties
                MQW->>MQW: Busca missão por MissionId

                alt UserGotAward == true
                    MQW->>MQW: Conclui missão (Status = Completed)
                    Note over MQW: TODO: disparar email/push<br/>parabenizando o usuário
                    MQW-->>SB: CompleteMessageAsync
                else UserGotAward == false
                    Note over MQW: Score insuficiente\nMissão não concluída
                    MQW-->>-SB: CompleteMessageAsync
                end
            and Benefício Quiz
                SB->>+QBW: Forward → queue: quiz-benefit-queue
                Note over QBW: Processa benefício\n(gamificação, pontos, badges)
                QBW-->>-SB: CompleteMessageAsync
            end

        else Sem MissionId
            API->>+SB: Publish → topic: quiz-answered\nBody: {QuizId, UserId, QuizScore, UserScore, UserGotAward}\n(sem ApplicationProperties)
            SB-->>-API: Accepted

            Note over SB: mission-quiz-filter não satisfeito\nquiz-benefit-filter captura (1=1)

            SB->>+QBW: Forward → queue: quiz-benefit-queue
            QBW-->>-SB: CompleteMessageAsync
        end

        API-->>-Client: 200 OK\n{QuizId, UserId, QuizScore, UserScore, UserGotAward}
    end
```

## Cálculo de Score

```mermaid
flowchart TD
    A["Recebe Answers[]"] --> B["Para cada Answer"]
    B --> C{"Alternativa existe\ne IsRightAnswer?"}
    C -->|Sim| D["userScore += Alternative.Score"]
    C -->|Não| E["userScore += 0"]
    D --> F["Próxima resposta"]
    E --> F
    F --> G{"Mais respostas?"}
    G -->|Sim| B
    G -->|Não| H["quizScore = soma de Score\nde todas as alternativas corretas"]
    H --> I{"HasAward?"}
    I -->|Não| J["userGotAward = false"]
    I -->|Sim| K{"HasMinimalPercentage?"}
    K -->|Não| L["userGotAward = true"]
    K -->|Sim| M{"userScore / quizScore\n* 100 >= MinimalPercentage?"}
    M -->|Sim| L
    M -->|Não| J
```

## Cenários

| Cenário | `userGotAward` | Worker de missão |
|---|---|---|
| Score abaixo do mínimo | `false` | Missão permanece `Active` |
| Score acima do mínimo + `HasAward = true` | `true` | Missão → `Completed` |
| Quiz sem `HasAward` | `false` | Missão permanece `Active` |
| Sem `MissionId` no payload | — | `Mission.QuizCompleteAnalyzer` não é acionado |
