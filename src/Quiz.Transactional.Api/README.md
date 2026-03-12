# Quiz.Transactional.Api

API transacional responsável por expor as operações relacionadas a quizzes no contexto do Azure Brasil Summit 2026.

## Responsabilidades

- Receber as respostas de um usuário para um quiz específico via endpoint REST
- Calcular a pontuação do usuário (`userScore`) com base nas alternativas corretas de cada questão
- Determinar se o usuário atingiu a pontuação mínima configurada no quiz (`HasMinimalPercentage` / `MinimalPercentage`) para obtenção do prêmio (`userGotAward`)
- Publicar o evento `quiz-answered` no tópico do Azure Service Bus com o resultado da tentativa
- Incluir a propriedade `MissionId` nas `ApplicationProperties` da mensagem quando presente no `DataDictionary` do payload, permitindo que o Service Bus aplique filtros e roteie a mensagem para as filas corretas

## Arquitetura

Utiliza **Vertical Slice Architecture (VSA)** com **Minimal API** do ASP.NET Core. Cada funcionalidade é encapsulada em uma fatia independente dentro de `Features/`, contendo request, handler e endpoint.

```
Features/
└── Quizzes/
    └── AnswerQuiz/
        ├── Quiz.cs                        # Modelo de domínio do quiz
        ├── Question.cs                    # Modelo de domínio da questão
        ├── Alternative.cs                 # Alternativa com score, nível e flag de resposta correta
        ├── AnswerQuizRequest.cs           # DTO de entrada (UserId, Answers[], DataDictionary)
        ├── AnswerQuizHandler.cs           # Lógica de negócio e cálculo de score
        └── AnswerQuizEndpoint.cs          # Registro da rota: POST /quizzes/{id}/answers
Infrastructure/
├── Data/
│   └── QuizStore.cs                      # Repositório in-memory com 2 quizzes mockados
└── Messaging/
    └── ServiceBusPublisher.cs            # Publicação no tópico Azure Service Bus
```

## Endpoint

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/quizzes/{id}/answers` | Submete as respostas do usuário para um quiz |

### Payload de entrada

```json
{
  "userId": "guid",
  "answers": [
    { "questionId": "guid", "alternativeId": "guid" }
  ],
  "dataDictionary": [
    { "MissionId": "guid-da-missao" }
  ]
}
```

### Evento publicado — tópico `quiz-answered`

**Body:**
```json
{
  "quizId": "guid",
  "userId": "guid",
  "quizScore": 30,
  "userScore": 20,
  "userGotAward": true
}
```

**ApplicationProperties (quando presentes):**
| Propriedade | Origem |
|---|---|
| `MissionId` | `DataDictionary["MissionId"]` |

## Configuração

```json
// appsettings.json
{
  "ConnectionStrings": {
    "AzureServiceBus": "<connection-string>"
  },
  "ServiceBus": {
    "QuizAnsweredTopic": "quiz-answered"
  }
}
```

## Quizzes mockados

| ID | Nome | Score máximo | % mínimo |
|----|------|-------------|----------|
| `11111111-…-0001` | Azure Fundamentals Challenge | 30 pts | 70% |
| `11111111-…-0002` | Cloud Architecture Patterns | 30 pts | 60% |
