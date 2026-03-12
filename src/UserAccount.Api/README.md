# UserAccount.Api

API transacional responsável pelo cadastro de contas de usuário no contexto do Azure Brasil Summit 2026.

## Responsabilidades

- Receber e validar dados de criação de conta de usuário via endpoint REST
- Persistir a conta no banco de dados PostgreSQL utilizando Entity Framework Core
- Publicar o evento `user-account-created` no tópico do Azure Service Bus após a criação bem-sucedida
- Inspecionar o `DataDictionary` do payload em busca das propriedades `CampaignId` e `IndicationToken`; quando ambas estiverem presentes, adicioná-las nas `ApplicationProperties` da mensagem, permitindo que o Service Bus aplique filtros de assinatura para rotear a mensagem às filas de destino corretas (ex.: fila MGM)

## Arquitetura

Utiliza **Vertical Slice Architecture (VSA)** com **Minimal API** do ASP.NET Core. Cada funcionalidade é encapsulada em uma fatia independente dentro de `Features/`, contendo request, handler, configuração de entidade e endpoint.

```
Features/
└── Accounts/
    └── CreateAccount/
        ├── CreateAccountRequest.cs             # DTO de entrada (Name, Email, DataDictionary)
        ├── UserAccountEntity.cs                # Entidade EF Core
        ├── UserAccountEntityConfiguration.cs  # Configuração Fluent API (jsonb para DataDictionary)
        ├── CreateAccountHandler.cs             # Lógica de negócio, persistência e extração de campanha
        └── CreateAccountEndpoint.cs            # Registro da rota: POST /accounts
Infrastructure/
├── Persistence/
│   └── AppDbContext.cs                         # DbContext com ApplyConfigurationsFromAssembly
└── Messaging/
    └── ServiceBusPublisher.cs                  # Publicação com suporte a ApplicationProperties
```

## Endpoint

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/accounts` | Cria uma nova conta de usuário |

### Payload de entrada

```json
{
  "name": "João Silva",
  "email": "joao@email.com",
  "dataDictionary": [
    { "CampaignId": "guid-da-campanha" },
    { "IndicationToken": "token-do-indicador" }
  ]
}
```

### Evento publicado — tópico `user-account-created`

**Body:**
```json
{
  "id": "guid",
  "name": "João Silva",
  "email": "joao@email.com",
  "createdAt": "2026-03-12T00:00:00Z"
}
```

**ApplicationProperties (quando `CampaignId` e `IndicationToken` estiverem preenchidos):**
| Propriedade | Origem |
|---|---|
| `CampaignId` | `DataDictionary["CampaignId"]` |
| `IndicationToken` | `DataDictionary["IndicationToken"]` |

## Configuração

```json
// appsettings.json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=user_account_db;Username=postgres;Password=...",
    "AzureServiceBus": "<connection-string>"
  },
  "ServiceBus": {
    "TopicName": "user-account-created"
  }
}
```
