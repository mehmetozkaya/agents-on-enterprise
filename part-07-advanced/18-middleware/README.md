# Chapter 18: Agent Middleware

<div align="center">

*Enterprise middleware patterns—guardrails, PII redaction, rate limiting*

[![Get the Full Book](https://img.shields.io/badge/📘_Full_Chapter_in_Book-agentsonenterprise.com-blue?style=flat-square)](https://agentsonenterprise.com/)

</div>

---

## 🎯 What You'll Learn

This chapter shows you how to build enterprise-grade middleware pipelines that protect, audit, and control your AI agents.

### Topics Covered

- **Guardrails** — Content filtering and safety controls
- **PII Redaction** — Automatic detection and masking of sensitive data
- **Rate Limiting** — Preventing abuse and controlling costs
- **Token Auditing** — Tracking usage for compliance and billing

---

## 📁 Code Examples

| Example | Description | Key Concepts |
|:---|:---|:---|
| [FinanceAgentMiddleware](./FinanceAgentMiddleware/) | Complete middleware pipeline | Guardrails, PII, rate limiting, auditing |

---

## 🚀 Quick Start

```bash
# Run the finance agent with middleware
cd part-07-advanced/18-middleware/FinanceAgentMiddleware
dotnet run
```

---

## 💡 Key Pattern: Middleware Pipeline

```csharp
// Build a middleware pipeline
var agent = chatClient
    .AsAIAgent(instructions: "You are a financial advisor.")
    .Use(new GuardrailMiddleware())      // Content safety
    .Use(new PIIRedactionMiddleware())   // Data protection
    .Use(new RateLimitingMiddleware())   // Abuse prevention
    .Use(new TokenAuditingMiddleware()); // Usage tracking

// All requests pass through the pipeline
var response = await agent.RunAsync(userInput);
```

---

## 🏗️ Middleware Architecture

```
Request ─▶ Guardrails ─▶ PII Redaction ─▶ Rate Limit ─▶ Agent
                                                          │
                                                          ▼
Response ◀─ Audit ◀───── PII Restore ◀──── Guardrails ◀─┘
```

---

## 🔒 Enterprise Considerations

| Concern | Middleware Solution |
|:---|:---|
| **Compliance** | PII redaction, audit logging |
| **Security** | Guardrails, input validation |
| **Cost Control** | Rate limiting, token tracking |
| **Reliability** | Circuit breakers, retry policies |

---

## 📖 Continue Reading

This example demonstrates the pattern. The complete chapter includes:

- 🛡️ Advanced guardrail strategies
- 🔐 Enterprise PII detection patterns
- 📊 Cost management and optimization
- 🏢 Compliance and audit requirements

<div align="center">

[![Get the Book](https://img.shields.io/badge/Get%20the%20Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

</div>

---

[← Back to Part VII](../README.md) | [Next: Skills →](../19-skills/README.md)
