# Topologia do Azure Service Bus

## Diagrama de Tópicos, Assinaturas e Filas

```mermaid
graph LR
    subgraph Publishers["Publishers"]
        UA["UserAccount.Api"]
        QT["Quiz.Transactional.Api"]
    end

    subgraph Topics["Tópicos"]
        T1["📨 user-account-created"]
        T2["📨 quiz-answered"]
    end

    subgraph Subscriptions["Assinaturas com Filtros"]
        subgraph T1Subs["Assinaturas de user-account-created"]
            S1["mgm-filter\n────────────────\nFILTRO SQL:\nCampaignId IS NOT NULL\nAND IndicationToken IS NOT NULL"]
        end
        subgraph T2Subs["Assinaturas de quiz-answered"]
            S2["mission-quiz-filter\n────────────────\nFILTRO SQL:\nMissionId IS NOT NULL"]
            S3["quiz-benefit-filter\n────────────────\nFILTRO SQL:\n1=1 (todas as mensagens)"]
        end
    end

    subgraph Queues["Filas de Destino"]
        Q1["📥 mgm-user-account\n-added-analyzer"]
        Q2["📥 mission-quiz\n-answer-analyzer"]
        Q3["📥 quiz-benefit\n-queue"]
    end

    subgraph Consumers["Consumers"]
        W1["Mission.MgmCompleteAnalyzer\n.Worker"]
        W2["Mission.QuizCompleteAnalyzer\n.Worker"]
        W3["Quiz.Benefit.Worker"]
    end

    UA -->|"Body: {Id, Name, Email, CreatedAt}\nProps: CampaignId?, IndicationToken?"| T1
    QT -->|"Body: {QuizId, UserId, QuizScore, UserScore, UserGotAward}\nProps: MissionId?"| T2

    T1 --> S1
    T2 --> S2
    T2 --> S3

    S1 -->|"Forward"| Q1
    S2 -->|"Forward"| Q2
    S3 -->|"Forward"| Q3

    Q1 --> W1
    Q2 --> W2
    Q3 --> W3
```

## Propriedades das Mensagens por Tópico

### Tópico `user-account-created`

| Propriedade | Tipo | Quando presente | Usado por |
|---|---|---|---|
| `CampaignId` | `Guid` | Quando o `DataDictionary` contém `CampaignId` | Filtro `mgm-filter` |
| `IndicationToken` | `string` | Quando o `DataDictionary` contém `IndicationToken` | Filtro `mgm-filter` |

> **Regra de negócio:** Apenas mensagens com **ambas** as propriedades (`CampaignId` AND `IndicationToken`) são encaminhadas para a fila MGM.

### Tópico `quiz-answered`

| Propriedade | Tipo | Quando presente | Usado por |
|---|---|---|---|
| `MissionId` | `Guid` | Quando o `DataDictionary` contém `MissionId` | Filtro `mission-quiz-filter` |

> **Regra de negócio:** Mensagens sem `MissionId` ainda chegam ao `Quiz.Benefit.Worker` (filtro `1=1`), mas são ignoradas pelo `Mission.QuizCompleteAnalyzer.Worker`.

## Settlement de Mensagens

Todos os workers utilizam `AutoCompleteMessages = false` e fazem o settlement explícito:

| Situação | Ação |
|---|---|
| Processamento bem-sucedido | `CompleteMessageAsync` |
| Payload inválido / não deserializável | `DeadLetterMessageAsync` |
| Entidade não encontrada | `DeadLetterMessageAsync` |
| Skip intencional (ex: score insuficiente) | `CompleteMessageAsync` |
