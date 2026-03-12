# Mission.MgmCompleteAnalyzer.Worker

Worker responsável por analisar indicações de novos usuários e determinar a conclusão de missões do tipo **MGM (Member Get Member)** no contexto do Azure Brasil Summit 2026.

## Responsabilidades

- Consumir mensagens da fila `mgm-user-account-added-analyzer` do Azure Service Bus
- Validar defensivamente a presença das propriedades `CampaignId` e `IndicationToken` nas `ApplicationProperties` da mensagem (garantidas pelo filtro do Service Bus, mas verificadas por segurança)
- Localizar a missão MGM ativa associada ao `CampaignId` recebido
- Atualizar o status da missão para `Completed` ao identificar que um novo usuário foi indicado com sucesso
- Sinalizar (via TODO) o ponto de disparo de notificação (email/push) ao referenciador identificado pelo `IndicationToken`

## Origem das mensagens

As mensagens consumidas por este worker são originadas pelo evento publicado pela **UserAccount.Api** no tópico `user-account-created`. O Azure Service Bus aplica um **filtro de assinatura** baseado na presença simultânea das propriedades `CampaignId` e `IndicationToken` nas `ApplicationProperties` e encaminha apenas as mensagens elegíveis para a fila `mgm-user-account-added-analyzer`.

## Arquitetura

Worker implementado como `BackgroundService` do .NET utilizando o SDK do **Azure Service Bus** (`ServiceBusProcessor`) com controle manual de settlement (`AutoCompleteMessages = false`).

```
Domain/
└── Mission.cs                        # Entidade de missão com enums ChallengeType e MissionStatus
Infrastructure/Data/
└── MissionStore.cs                   # Repositório in-memory com missões MGM mockadas
Messaging/
└── UserAccountAddedMessage.cs        # Contrato do evento publicado pela UserAccount.Api
Worker.cs                             # Consumer: validação de propriedades, lookup e conclusão de missão
```

## Fluxo de processamento

```
Mensagem recebida
    │
    ├── Desserialização falhou?              → Dead-letter (InvalidPayload)
    ├── CampaignId ausente/inválido?         → Dead-letter (MissingCampaignId)
    ├── IndicationToken ausente/vazio?       → Dead-letter (MissingIndicationToken)
    ├── Missão MGM não encontrada?           → Dead-letter (MissionNotFound)
    └── Missão encontrada                    → Complete missão + log + TODO notificação ao referenciador
```

## Contrato da mensagem

**Body:**
```json
{
  "id": "guid",
  "name": "João Silva",
  "email": "joao@email.com",
  "createdAt": "2026-03-12T00:00:00Z"
}
```

**ApplicationProperties (filtradas pelo Service Bus):**
| Propriedade | Tipo | Descrição |
|---|---|---|
| `CampaignId` | `Guid` | Identificador da campanha MGM |
| `IndicationToken` | `string` | Token que identifica o usuário referenciador |

## Configuração

```json
// appsettings.json
{
  "ConnectionStrings": {
    "AzureServiceBus": "<connection-string>"
  },
  "ServiceBus": {
    "QueueName": "mgm-user-account-added-analyzer"
  }
}
```

## Missões mockadas

| ID | Nome | Campanha |
|----|------|----------|
| `55555555-…-0001` | Indique um Amigo - Summit 2026 | `cccccccc-…-0001` |
| `55555555-…-0002` | Embaixador da Comunidade | `cccccccc-…-0002` |
