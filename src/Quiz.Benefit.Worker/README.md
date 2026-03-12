# Quiz.Benefit.Worker

Worker responsável por processar os benefícios concedidos aos usuários que completaram um quiz com sucesso no contexto do Azure Brasil Summit 2026.

## Responsabilidades

- Consumir mensagens da fila do Azure Service Bus destinada ao processamento de benefícios de quiz
- Processar a concessão de benefícios (ex.: pontos, badges, recompensas de gamificação) para usuários que atingiram o score mínimo em um quiz
- Atuar de forma assíncrona e desacoplada, garantindo resiliência no processamento de recompensas

## Origem das mensagens

As mensagens consumidas por este worker são originadas pelo evento publicado pela **Quiz.Transactional.Api** no tópico `quiz-answered`. O Azure Service Bus aplica um filtro na assinatura do tópico e encaminha as mensagens elegíveis para a fila correspondente a este worker.

## Arquitetura

Worker implementado como `BackgroundService` do .NET utilizando o SDK do **Azure Service Bus** (`ServiceBusProcessor`) para consumo assíncrono de mensagens com controle manual de settlement (`AutoCompleteMessages = false`).

## Configuração

```json
// appsettings.json
{
  "ConnectionStrings": {
    "AzureServiceBus": "<connection-string>"
  },
  "ServiceBus": {
    "QueueName": "<nome-da-fila>"
  }
}
```

> **Nota:** Este worker ainda está em fase inicial de implementação. A lógica de processamento de benefícios deve ser adicionada ao método `OnMessageReceivedAsync` da classe `Worker`.
