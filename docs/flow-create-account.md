# Fluxo — Criação de Conta de Usuário

## Sequência Completa

```mermaid
sequenceDiagram
    actor Client as 👤 Client
    participant API as UserAccount.Api
    participant DB as PostgreSQL
    participant SB as Azure Service Bus
    participant MGM as Mission.MgmCompleteAnalyzer.Worker

    Client->>+API: POST /accounts\n{Name, Email, DataDictionary}

    API->>API: Persiste UserAccountEntity\n(Id, Name, Email, DataDictionary as jsonb, CreatedAt)
    API->>+DB: SaveChangesAsync()
    DB-->>-API: OK

    API->>API: ExtractCampaignProperties(DataDictionary)\nBusca CampaignId e IndicationToken

    alt CampaignId e IndicationToken presentes
        API->>+SB: Publish → topic: user-account-created\nBody: {Id, Name, Email, CreatedAt}\nProps: {CampaignId, IndicationToken}
        SB-->>-API: Accepted

        Note over SB: Filtro mgm-filter:<br/>CampaignId IS NOT NULL<br/>AND IndicationToken IS NOT NULL

        SB->>+MGM: Forward → queue: mgm-user-account-added-analyzer
        MGM->>MGM: Valida CampaignId e IndicationToken
        MGM->>MGM: Busca missão MGM ativa por CampaignId
        MGM->>MGM: Conclui missão (Status = Completed)
        Note over MGM: TODO: disparar email/push<br/>para o referenciador (IndicationToken)
        MGM-->>-SB: CompleteMessageAsync
    else Sem CampaignId ou IndicationToken
        API->>+SB: Publish → topic: user-account-created\nBody: {Id, Name, Email, CreatedAt}\n(sem ApplicationProperties)
        SB-->>-API: Accepted
        Note over SB: Nenhuma assinatura captura a mensagem\n(filtro mgm-filter não satisfeito)
    end

    API-->>-Client: 201 Created\n{id, name, email, createdAt, dataDictionary}
```

## Cenários

| Cenário | Resultado |
|---|---|
| `DataDictionary` sem `CampaignId`/`IndicationToken` | Conta criada, evento publicado sem properties, nenhum worker processa |
| `DataDictionary` com ambas as propriedades | Conta criada + missão MGM concluída pelo worker |
| Falha na persistência no PostgreSQL | Retorna `500`, nenhum evento publicado |
