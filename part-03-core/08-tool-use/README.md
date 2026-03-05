# Chapter 08: Tool Use & Function Calling

<div align="center">

*Function calling, code interpretation, file search, and web search capabilities*

[![Get the Full Book](https://img.shields.io/badge/📘_Full_Chapter_in_Book-agentsonenterprise.com-blue?style=flat-square)](https://agentsonenterprise.com/)

</div>

---

## 🎯 What You'll Learn

Tools transform agents from conversational interfaces into **autonomous systems** that can interact with the real world.

### Topics Covered

- **Function Calling** — Exposing C# methods as agent tools
- **Human-in-the-Loop** — Approval workflows for sensitive operations
- **Code Interpreter** — Dynamic code execution capabilities
- **File Search** — Searching and extracting from documents
- **Web Search** — Internet search integration

---

## 📁 Code Examples

| Example | Description | Key Concepts |
|:---|:---|:---|
| [FunctionCall](./FunctionCall/) | Basic function calling | Tool definition, invocation |
| [ApproveRequiredFunc](./ApproveRequiredFunc/) | Human approval workflows | Security, confirmation patterns |
| [CodeInterpreter](./CodeInterpreter/) | Execute code dynamically | Sandboxing, dynamic execution |
| [FileSearch](./FileSearch/) | Search file contents | Document parsing, extraction |
| [WebSearch](./WebSearch/) | Internet search capability | External API integration |

---

## 🚀 Quick Start

```bash
# Run the function calling example
cd part-03-core/08-tool-use/FunctionCall
dotnet run

# Try human-in-the-loop approval
cd ../ApproveRequiredFunc
dotnet run
```

---

## 💡 Key Pattern: Function Calling

```csharp
// Define a tool as a C# method
[Description("Gets the current weather for a location")]
public static string GetWeather(string location)
{
    return $"Weather in {location}: 72°F, Sunny";
}

// Register tools with the agent
var agent = client
    .GetChatClient(model)
    .AsAIAgent(instructions: "You are a weather assistant.")
    .WithTools([GetWeather]);
```

---

## 🔒 Enterprise Considerations

| Concern | Solution |
|:---|:---|
| **Security** | Approve sensitive operations before execution |
| **Auditing** | Log all tool invocations for compliance |
| **Rate Limiting** | Prevent abuse of external APIs |
| **Error Handling** | Graceful degradation when tools fail |

---

## 📖 Continue Reading

These examples demonstrate the patterns. The complete chapter includes:

- 🔧 Advanced tool design patterns
- 🛡️ Security best practices for tool execution
- 📊 Tool performance optimization
- 🏢 Enterprise tool governance

<div align="center">

[![Get the Book](https://img.shields.io/badge/Get%20the%20Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

</div>

---

[← Previous: Getting Started](../07-getting-started/README.md) | [Next: Memory →](../09-memory/README.md)
