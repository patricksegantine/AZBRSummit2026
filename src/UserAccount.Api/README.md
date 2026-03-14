# UserAccount.Api

API transacional responsável pelo gerenciamento de contas de usuário no contexto do Azure Brasil Summit 2026.

## Responsabilidades

- Receber e validar dados de criação de conta via endpoint REST
- Persistir a conta no banco de dados PostgreSQL utilizando Entity Framework Core
- Consultar contas existentes por `Id`, `Email` ou `Cpf`
- Atualizar dados de uma conta existente
- Publicar eventos no tópico `user-account-added-or-updated` do Azure Service Bus após cada operação de escrita
- Inspecionar o `DataDictionary` do payload em busca das propriedades `CampaignId`, `IndicationToken` e `MissionId`; quando presentes, adicioná-las nas `ApplicationProperties` da mensagem, permitindo que o Service Bus aplique filtros de assinatura para rotear a mensagem às filas de destino corretas

## Arquitetura

Utiliza **Vertical Slice Architecture (VSA)** com **Minimal API** do ASP.NET Core. Cada funcionalidade é encapsulada em uma fatia independente dentro de `Features/`, contendo request, handler e endpoint.

```
Features/
└── Accounts/
    ├── CreateAccount/
    │   ├── CreateAccountRequest.cs             # DTO de entrada (Name, Email, PhoneNumber, Cpf, DataDictionary)
    │   ├── UserAccountEntity.cs                # Entidade EF Core
    │   ├── UserAccountEntityConfiguration.cs  # Configuração Fluent API (jsonb para DataDictionary)
    │   ├── CreateAccountHandler.cs             # Persistência + extração de propriedades de campanha
    │   └── CreateAccountEndpoint.cs            # Rota: POST /accounts
    ├── GetAccount/
    │   ├── GetAccountsQuery.cs                 # Query: Id?, Email?, Cpf?
    │   ├── GetAccountsHandler.cs               # Consulta filtrada no DbContext
    │   ├── GetAccountResponse.cs               # DTO de resposta
    │   └── GetAccountsEndpoint.cs              # Rota: GET /accounts
    └── UpdateAccount/
        ├── UpdateAccountRequest.cs             # DTO de entrada (Name, Email, PhoneNumber, DataDictionary)
        ├── UpdateAccountHandler.cs             # Atualização + publicação de evento
        └── UpdateAccountEndpoint.cs            # Rota: PUT /accounts/{id}
Infrastructure/
├── Persistence/
│   └── AppDbContext.cs                         # DbContext com ApplyConfigurationsFromAssembly
└── Messaging/
    └── ServiceBusPublisher.cs                  # Publicação com suporte a ApplicationProperties
```

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/accounts` | Cria uma nova conta de usuário |
| `GET` | `/accounts` | Consulta conta por `id`, `email` ou `cpf` (query params) |
| `PUT` | `/accounts/{id}` | Atualiza dados de uma conta existente |

### POST /accounts — Payload de entrada

```json
{
  "name": "João Silva",
  "email": "joao@email.com",
  "phoneNumber": "+5511999999999",
  "cpf": "123.456.789-00",
  "dataDictionary": [
    { "CampaignId": "guid-da-campanha" },
    { "IndicationToken": "token-do-indicador" },
    { "MissionId": "guid-da-missao" }
  ]
}
```

### GET /accounts — Query params

| Parâmetro | Tipo | Descrição |
|-----------|------|-----------|
| `id` | `Guid?` | Filtrar por ID da conta |
| `email` | `string?` | Filtrar por e-mail |
| `cpf` | `string?` | Filtrar por CPF |

### PUT /accounts/{id} — Payload de entrada

```json
{
  "name": "João Silva Atualizado",
  "email": "joao.novo@email.com",
  "phoneNumber": "+5511988888888",
  "dataDictionary": []
}
```

### Evento publicado — tópico `user-account-added-or-updated`

**Body:**
```json
{
  "id": "guid",
  "name": "João Silva",
  "email": "joao@email.com",
  "createdAt": "2026-03-12T00:00:00Z"
}
```

**ApplicationProperties:**
| Propriedade | Origem | Quando presente |
|---|---|---|
| `EventName` | `"user-account-created"` ou `"user-account-updated"` | Sempre |
| `CampaignId` | `DataDictionary["CampaignId"]` | Apenas no POST, quando informado |
| `IndicationToken` | `DataDictionary["IndicationToken"]` | Apenas no POST, quando informado |
| `MissionId` | `DataDictionary["MissionId"]` | Apenas no POST, quando informado |

## Configuração

```json
// appsettings.json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=user_account_db;Username=postgres;Password=...",
    "AzureServiceBus": "<connection-string>"
  },
  "ServiceBus": {
    "TopicName": "user-account-added-or-updated"
  }
}
```
