# Referências e Recursos

Documentação oficial, tutoriais e exemplos utilizados como base para o desenvolvimento desta solução.

---

## Azure Service Bus

### Documentação Oficial (Microsoft Learn)

| Recurso | Descrição |
|---|---|
| [Visão geral do Azure Service Bus](https://learn.microsoft.com/pt-br/azure/service-bus-messaging/service-bus-messaging-overview) | Introdução ao Azure Service Bus: conceitos de filas, tópicos, assinaturas e casos de uso |
| [Service Bus vs Azure Storage Queues](https://learn.microsoft.com/pt-br/azure/service-bus-messaging/service-bus-azure-and-service-bus-queues-compared-contrasted) | Comparação detalhada entre Azure Service Bus Queues e Azure Storage Queues — quando usar cada um |
| [Exemplos de filtros de assinatura](https://learn.microsoft.com/pt-br/azure/service-bus-messaging/service-bus-filter-examples) | Exemplos práticos de filtros SQL e correlação em assinaturas de tópicos — base para os filtros utilizados nesta solução |
| [Exemplos de código — Service Bus](https://learn.microsoft.com/pt-br/azure/service-bus-messaging/service-bus-samples) | Repositório oficial com exemplos de envio, recebimento, sessões, dead-letter e mais |

### Relação com esta solução

Os filtros SQL aplicados nas assinaturas desta solução seguem os padrões documentados em **Exemplos de filtros de assinatura**:

```sql
-- Assinatura mgm-filter (tópico: user-account-created)
CampaignId IS NOT NULL AND IndicationToken IS NOT NULL

-- Assinatura mission-quiz-filter (tópico: quiz-answered)
MissionId IS NOT NULL
```

---

## Tutoriais em Vídeo

### Azure Service Bus Local Emulator

> Aprenda a executar o Azure Service Bus localmente sem necessidade de uma conta Azure, ideal para desenvolvimento e testes.

[![Azure Service Bus Local Emulator](https://img.shields.io/badge/YouTube-Tutorial-red?logo=youtube)](https://www.youtube.com/watch?v=QUlBY11-AV4)

**[Como usar o Azure Service Bus Emulator localmente](https://www.youtube.com/watch?v=QUlBY11-AV4)**

Conteúdo abordado no vídeo:
- Instalação e configuração do emulador local do Service Bus
- Criação de filas e tópicos no ambiente local
- Integração com aplicações .NET usando a mesma connection string

#### Configuração rápida com o emulador

Para usar o emulador localmente em vez de um namespace Azure real, atualize a connection string nos projetos:

```json
{
  "ConnectionStrings": {
    "AzureServiceBus": "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;"
  }
}
```

---

## Conceitos-chave desta solução

### Tópicos e Assinaturas

Um **tópico** recebe mensagens de publicadores. Cada **assinatura** é uma cópia independente do tópico — múltiplos consumidores podem receber a mesma mensagem com filtros diferentes.

```
Publisher → Topic → Subscription A (filtro X) → Consumer A
                 → Subscription B (filtro Y) → Consumer B
                 → Subscription C (sem filtro) → Consumer C
```

→ Documentação: [Tópicos e assinaturas do Service Bus](https://learn.microsoft.com/pt-br/azure/service-bus-messaging/service-bus-messaging-overview#topics-and-subscriptions)

### Filtros SQL em Assinaturas

Permitem rotear mensagens com base em **propriedades da mensagem** (`ApplicationProperties`) usando sintaxe SQL-like, sem lógica no publicador ou consumidor.

```sql
-- Exemplos de expressões válidas
CampaignId IS NOT NULL
UserScore > 20
EventType = 'OrderCreated'
Region IN ('BRA', 'ARG') AND Priority > 1
```

→ Documentação: [Filtros e ações de assinatura](https://learn.microsoft.com/pt-br/azure/service-bus-messaging/service-bus-filter-examples)

### Dead-Letter Queue

Cada fila e assinatura possui uma **Dead-Letter Queue (DLQ)** automática. Mensagens são movidas para ela quando:
- Excedem o número máximo de entregas (`MaxDeliveryCount`)
- São explicitamente dead-letteradas pela aplicação (`DeadLetterMessageAsync`)
- Expiram o TTL

Nesta solução, todos os workers usam `DeadLetterMessageAsync` para mensagens inválidas ou com dados ausentes.

→ Documentação: [Dead-letter queues](https://learn.microsoft.com/pt-br/azure/service-bus-messaging/service-bus-dead-letter-queues)

### Forwarding (Auto-forward)

As assinaturas desta solução usam **auto-forward** para encaminhar mensagens filtradas diretamente para filas dedicadas, desacoplando os workers dos tópicos:

```
Topic: quiz-answered
  └── Subscription: mission-quiz-filter (SQL: MissionId IS NOT NULL)
        └── forward-to → Queue: mission-quiz-answer-analyzer
              └── Mission.QuizCompleteAnalyzer.Worker (consome da fila)
```

→ Documentação: [Encadeamento de entidades com auto-forwarding](https://learn.microsoft.com/pt-br/azure/service-bus-messaging/service-bus-auto-forwarding)
