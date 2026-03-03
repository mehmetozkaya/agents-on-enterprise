# Agent Middleware Examples

Demonstrates middleware patterns for **Microsoft Agent Framework** using `DelegatingChatClient`.

## Middleware Pipeline

```
User Request → Guardrail → PII → RateLimit → TokenAudit → LLM → Response
```

Each middleware wraps the next, creating an **onion architecture** for cross-cutting concerns.

## How Middleware Wiring Works

### The Key Concept: Each middleware wraps the previous one

```csharp
IChatClient pipeline = baseLLMClient;
pipeline = new TokenAuditingMiddleware(pipeline);   // Wraps LLM
pipeline = new RateLimitingMiddleware(pipeline);    // Wraps TokenAudit
pipeline = new PIIRedactionMiddleware(pipeline);    // Wraps RateLimit
pipeline = new GuardrailMiddleware(pipeline);       // Wraps PII (outermost)
```

This creates a nested structure (onion architecture):

```
┌─────────────────────────────────────┐
│ GuardrailMiddleware                 │ ← Request enters here first
│  ┌─────────────────────────────────┐│
│  │ PIIRedactionMiddleware          ││
│  │  ┌─────────────────────────────┐││
│  │  │ RateLimitingMiddleware      │││
│  │  │  ┌─────────────────────────┐│││
│  │  │  │ TokenAuditingMiddleware ││││
│  │  │  │  ┌─────────────────────┐││││
│  │  │  │  │ Azure OpenAI Client │││││ ← Actual LLM call
│  │  │  │  └─────────────────────┘││││
│  │  │  └─────────────────────────┘│││
│  │  └─────────────────────────────┘││
│  └─────────────────────────────────┘│
└─────────────────────────────────────┘
```

## Middleware Components

### 1. TokenAuditingMiddleware
Logs token consumption for cost tracking.

```csharp
pipeline = new TokenAuditingMiddleware(pipeline);
// Output: [TokenAudit] Tokens -> Input: 50, Output: 120
```

### 2. PIIRedactionMiddleware  
Detects and redacts sensitive data (SSN, email, phone).

```csharp
pipeline = new PIIRedactionMiddleware(pipeline);
// Input:  "My SSN is 123-45-6789"
// Output: "My SSN is [SSN]"
```

### 3. GuardrailMiddleware
Blocks prompt injection and malicious content.

```csharp
pipeline = new GuardrailMiddleware(pipeline);
// Blocks: "ignore all previous instructions", "reveal system prompt"
```

### 4. RateLimitingMiddleware
Throttles requests per minute.

```csharp
pipeline = new RateLimitingMiddleware(pipeline, maxRequestsPerMinute: 10);
// Output: [RateLimit] Request allowed (5/10)
```

## Quick Start

```csharp
// Build middleware pipeline (order matters!)
IChatClient pipeline = new AzureOpenAIClient(endpoint, credential)
    .GetChatClient("gpt-4o")
    .AsIChatClient();

pipeline = new TokenAuditingMiddleware(pipeline);   // Innermost
pipeline = new RateLimitingMiddleware(pipeline);
pipeline = new PIIRedactionMiddleware(pipeline);
pipeline = new GuardrailMiddleware(pipeline);       // Outermost

// Create agent
AIAgent agent = pipeline.AsAIAgent(
    name: "MyAgent",
    instructions: "You are a helpful assistant."
);

var response = await agent.RunAsync("Hello!");
```

## Creating Custom Middleware

```csharp
public class MyMiddleware : DelegatingChatClient
{
    public MyMiddleware(IChatClient inner) : base(inner) { }

    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken ct = default)
    {
        // Pre-processing
        Console.WriteLine("[MyMiddleware] Before LLM call");

        var response = await base.GetResponseAsync(messages, options, ct);

        // Post-processing
        Console.WriteLine("[MyMiddleware] After LLM call");
        return response;
    }
}
```

## Project Structure

```
FinanceAgentMiddleware/
├── Program.cs                              # Demo app
├── TokenAuditingChatClient.cs              # Token auditing
└── Middleware/
    ├── PIIRedactionMiddleware.cs           # PII detection
    ├── GuardrailMiddleware.cs              # Security
    └── RateLimitingMiddleware.cs           # Throttling
```
