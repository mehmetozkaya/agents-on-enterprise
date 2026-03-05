# Part III: Core Agent Development

<div align="center">

*Mechanics, Tools, and Memory—the essential building blocks of production AI agents*

[![Get the Full Book](https://img.shields.io/badge/📘_Get_the_Full_Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

</div>

---

## 🎯 Overview

This is where you **get your hands dirty**. Part III covers the core mechanics of agent development—from basic agent creation to advanced tool integration and memory systems.

> **"An agent without tools is just a chatbot. An agent without memory forgets everything."**

---

## 📚 Chapters in This Part

| Chapter | Topic | Description | Examples |
|:---:|:---|:---|:---:|
| 07 | [Getting Started](./07-getting-started/README.md) | Build your first agents with streaming and structured outputs | 5 |
| 08 | [Tool Use](./08-tool-use/README.md) | Function calling, code interpreter, file & web search | 5 |
| 09 | [Memory](./09-memory/README.md) | Session management, context providers, persistent memory | 4 |

---

## 🔑 Key Takeaways

After completing this part, you will:

- ✅ Build **production-ready agents** with proper streaming and error handling
- ✅ Implement **tool/function calling** for external system integration
- ✅ Configure **memory systems** for context retention and personalization
- ✅ Understand **structured outputs** for reliable data extraction

---

## 📂 Code Examples in This Part

```
part-03-core/
├── 07-getting-started/
│   ├── BasicAgentApp/          # Minimal agent setup
│   ├── MultiTurn/              # Conversation handling
│   ├── Streaming/              # Real-time response streaming
│   ├── StructuredOutput/       # Type-safe outputs
│   └── StructuredOutputStreaming/
├── 08-tool-use/
│   ├── FunctionCall/           # Basic function calling
│   ├── ApproveRequiredFunc/    # Human-in-the-loop tools
│   ├── CodeInterpreter/        # Execute code dynamically
│   ├── FileSearch/             # Search file contents
│   └── WebSearch/              # Internet search capability
└── 09-memory/
    ├── BasicSessionExample/    # Session management
    ├── CustomContextProvider/  # Custom context injection
    ├── MockCosmosDb/           # Persistent memory
    └── TravelAgentWithMem0/    # Advanced memory patterns
```

---

## 🚀 Ready to Go Deeper?

This repository provides working code examples. For comprehensive explanations, architecture decisions, and enterprise patterns:

<div align="center">

[![Get the Book](https://img.shields.io/badge/Get%20the%20Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

**Complete Enterprise Agentic AI Architectures with .NET**

</div>

---

[← Back to Main README](../README.md) | [Previous: Part II](../part-02-explore/README.md) | [Next: Part IV →](../part-04-multi-agent/README.md)