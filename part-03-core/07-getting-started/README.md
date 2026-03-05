# Chapter 07: Getting Started with AI Agents

<div align="center">

*Hands-on guide to building your first AI agent with streaming and structured outputs*

[![Get the Full Book](https://img.shields.io/badge/📘_Full_Chapter_in_Book-agentsonenterprise.com-blue?style=flat-square)](https://agentsonenterprise.com/)

</div>

---

## 🎯 What You'll Learn

This chapter takes you from zero to production-ready agent patterns with hands-on code examples.

### Topics Covered

- **Basic Agent Creation** — Minimal setup for your first agent
- **Multi-Turn Conversations** — Maintaining context across interactions
- **Streaming Responses** — Real-time output for better UX
- **Structured Outputs** — Type-safe, validated responses

---

## 📁 Code Examples

| Example | Description | Key Concepts |
|:---|:---|:---|
| [BasicAgentApp](./BasicAgentApp/) | Minimal agent setup | Agent initialization, simple invocation |
| [MultiTurn](./MultiTurn/) | Conversation handling | Context management, chat history |
| [Streaming](./Streaming/) | Real-time response streaming | Async streams, progressive output |
| [StructuredOutput](./StructuredOutput/) | Type-safe outputs | JSON schema, validation |
| [StructuredOutputStreaming](./StructuredOutputStreaming/) | Streaming + structured | Combined patterns |

---

## 🚀 Quick Start

```bash
# Run the basic agent example
cd part-03-core/07-getting-started/BasicAgentApp
dotnet run

# Try streaming responses
cd ../Streaming
dotnet run
```

---

## 💡 Key Patterns

### Basic Agent
```csharp
// Minimal agent setup with Microsoft Agent Framework
AIAgent agent = new AzureOpenAIClient(
        new Uri(endpoint),        
        new AzureCliCredential())
    .GetChatClient(model)
    .AsAIAgent(instructions: "You are a helpful assistant.");

var response = await agent.RunAsync("Hello!");
```

### Streaming
```csharp
// Real-time streaming for better user experience
await foreach (var chunk in agent.RunStreamingAsync(prompt))
{
    Console.Write(chunk);
}
```

---

## 📖 Continue Reading

These examples demonstrate the patterns. The complete chapter includes:

- 🏗️ Architecture decisions and trade-offs
- ⚠️ Error handling and resilience patterns
- 📊 Performance optimization techniques
- 🏢 Enterprise deployment considerations

<div align="center">

[![Get the Book](https://img.shields.io/badge/Get%20the%20Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

</div>

---

[← Back to Part III](../README.md) | [Next: Tool Use →](../08-tool-use/README.md)
