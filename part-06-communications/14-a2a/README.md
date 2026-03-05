# Chapter 14: Agent-to-Agent (A2A) Protocol

<div align="center">

*Building enterprise compliance services with agent-to-agent protocols*

[![Get the Full Book](https://img.shields.io/badge/📘_Full_Chapter_in_Book-agentsonenterprise.com-blue?style=flat-square)](https://agentsonenterprise.com/)

</div>

---

## 🎯 What You'll Learn

This chapter covers the Agent-to-Agent (A2A) protocol for enabling autonomous agents to communicate and collaborate.

### Topics Covered

- **A2A Protocol Fundamentals** — How agents discover and communicate with each other
- **Agent Cards** — Self-describing agent capabilities
- **Task Delegation** — Passing work between specialized agents
- **Enterprise Integration** — A2A in microservices architectures

---

## 📁 Code Examples

| Example | Description | Key Concepts |
|:---|:---|:---|
| [EnterpriseComplianceService](./EnterpriseComplianceService/) | A2A-enabled compliance service | Agent discovery, task handling |

---

## 🚀 Quick Start

```bash
# Run the A2A compliance service
cd part-06-communications/14-a2a/EnterpriseComplianceService
dotnet run
```

---

## 💡 Key Pattern: A2A Communication

```csharp
// Agent exposes its capabilities via Agent Card
app.MapGet("/.well-known/agent.json", () => new AgentCard
{
    Name = "ComplianceAgent",
    Description = "Handles regulatory compliance checks",
    Capabilities = ["risk-assessment", "kyc-validation"]
});

// Agent handles tasks from other agents
app.MapPost("/tasks", async (AgentTask task) =>
{
    var result = await ProcessComplianceTask(task);
    return Results.Ok(result);
});
```

---

## 🔒 Enterprise Considerations

| Concern | Solution |
|:---|:---|
| **Discovery** | Service registry, DNS-based discovery |
| **Trust** | Mutual TLS, signed agent cards |
| **Governance** | Central policy enforcement |
| **Monitoring** | Distributed tracing across agents |

---

## 📖 Continue Reading

This example demonstrates the pattern. The complete chapter includes:

- 📜 Full A2A protocol specification
- 🔐 Security and trust models
- 🏗️ Multi-agent orchestration patterns
- 🏢 Enterprise deployment strategies

<div align="center">

[![Get the Book](https://img.shields.io/badge/Get%20the%20Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

</div>

---

[← Previous: Communications](../13-communications/README.md) | [Next: MCP →](../15-mcp/README.md)
