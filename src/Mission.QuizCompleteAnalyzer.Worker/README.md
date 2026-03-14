# Mission.QuizCompleteAnalyzer.Worker

Worker responsável por analisar as respostas de quizzes e determinar a conclusão de missões do tipo **Quiz** no contexto do Azure Brasil Summit 2026.

## Responsabilidades

- Consumir mensagens da fila `mission-quiz-answer-analyzer` do Azure Service Bus
- Verificar se o usuário atingiu o score mínimo do quiz (`UserGotAward`)
- Localizar a missão associada pelo `MissionId` presente nas `ApplicationProperties` da mensagem
- Atualizar o status da missão para `Completed` quando o usuário atingiu o critério de conclusão
- Registrar log detalhado de cada processamento, incluindo casos em que o score mínimo não foi atingido
- Sinalizar (via TODO) o ponto de disparo de notificação (email/push) ao usuário ao completar a missão

## Origem das mensagens

As mensagens consumidas por este worker são originadas pelo evento publicado pela **Quiz.Transactional.Api** no tópico `quiz-answered`. O Azure Service Bus aplica um filtro de assinatura baseado na presença da propriedade `MissionId` e encaminha as mensagens para a fila `mission-quiz-answer-analyzer`.

## Arquitetura

Worker implementado como `BackgroundService` do .NET utilizando o SDK do **Azure Service Bus** (`ServiceBusProcessor`) com controle manual de settlement (`AutoCompleteMessages = false`).

O domínio e a infraestrutura de missões são fornecidos pelos projetos compartilhados:
- **`Mission.Domain`** — entidades `Mission`, `UserMission` e `IndicationToken`
- **`Mission.Infrastructure`** — repositórios in-memory (`MissionStore`, `UserMissionStore`) e dados de seed

```
Handlers/
└── QuizCompletionHandler.cs          # Lógica de negócio: verificação de score e conclusão da missão Quiz
Messaging/
└── QuizAnsweredMessage.cs            # Contrato do evento publicado pela Quiz.Transactional.Api
Worker.cs                             # Consumer: validação, delegação ao handler e settlement
```

## Fluxo de processamento

```
Mensagem recebida
    │
    ├── Desserialização falhou?          → Dead-letter (InvalidPayload)
    ├── MissionId ausente/inválido?      → Complete (skip)
    ├── Missão não encontrada?           → Dead-letter (MissionNotFound)
    ├── UserGotAward == false?           → Complete (score insuficiente, log informativo)
    └── UserGotAward == true             → Complete missão + log + TODO notificação
```

## Contrato da mensagem

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

**ApplicationProperties:**
| Propriedade | Tipo | Descrição |
|---|---|---|
| `MissionId` | `Guid` | Identificador da missão a ser analisada |

## Configuração

```json
// appsettings.json
{
  "ConnectionStrings": {
    "AzureServiceBus": "<connection-string>"
  },
  "ServiceBus": {
    "QueueName": "mission-quiz-answer-analyzer"
  }
}
```

## Missões mockadas

| ID | Nome | Tipo | % mínimo |
|----|------|------|----------|
| `44444444-…-0001` | Azure Fundamentals Quiz Mission | Quiz | 70% |
| `44444444-…-0002` | Cloud Architecture Patterns Mission | Quiz | 60% |
