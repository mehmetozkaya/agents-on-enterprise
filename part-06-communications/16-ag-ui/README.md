# Chapter 16: AG-UI Protocol

<div align="center">

*Agent-to-UI communication patterns with client-server architecture*

[![Get the Full Book](https://img.shields.io/badge/📘_Full_Chapter_in_Book-agentsonenterprise.com-blue?style=flat-square)](https://agentsonenterprise.com/)

</div>

---

## 🎯 What You'll Learn

This chapter covers the AG-UI protocol for building real-time, agent-powered user interfaces.

### Topics Covered

- **AG-UI Fundamentals** — Protocol for agent-to-UI communication
- **Streaming Responses** — Real-time output to user interfaces
- **Client-Server Architecture** — Separating UI from agent logic
- **Event Handling** — User interactions and agent responses

---

## 📁 Code Examples

| Example | Description | Key Concepts |
|:---|:---|:---|
| [Server](./Server/) | AG-UI server implementation | API endpoints, streaming |
| [Client](./Client/) | AG-UI client application | UI integration, event handling |

---

## 🚀 Quick Start

```bash
# Start the server
cd part-06-communications/16-ag-ui/Server
dotnet run

# In another terminal, start the client
cd ../Client
dotnet run
```

---

## 💡 Key Pattern: AG-UI Streaming

```csharp
// Server: Stream agent responses
app.MapPost("/chat", async (ChatRequest request, HttpContext context) =>
{
    context.Response.ContentType = "text/event-stream";

    await foreach (var chunk in agent.RunStreamingAsync(request.Message))
    {
        await context.Response.WriteAsync($"data: {chunk}\n\n");
        await context.Response.Body.FlushAsync();
    }
});

// Client: Consume streaming responses
await foreach (var chunk in client.StreamChatAsync(message))
{
    Console.Write(chunk);
}
```

---

## 🔒 Enterprise Considerations

| Concern | Solution |
|:---|:---|
| **Latency** | Streaming for responsive UX |
| **Scalability** | Load-balanced agent backends |
| **Security** | User authentication, session management |
| **Accessibility** | Progressive rendering for all users |

---

## 📖 Continue Reading

These examples demonstrate the patterns. The complete chapter includes:

- 🎨 UI/UX best practices for agent interfaces
- 🔄 Error handling and retry strategies
- 📱 Mobile and web implementation patterns
- 🏢 Enterprise deployment considerations

<div align="center">

[![Get the Book](https://img.shields.io/badge/Get%20the%20Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

</div>

---

[← Previous: MCP](../15-mcp/README.md) | [Next: Developer UI →](../17-devui/README.md)
