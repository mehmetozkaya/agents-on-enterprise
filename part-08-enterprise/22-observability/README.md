# Chapter 22: Observability

<div align="center">

*Full observability with .NET Aspire and OpenTelemetry*

[![Get the Full Book](https://img.shields.io/badge/📘_Full_Chapter_in_Book-agentsonenterprise.com-blue?style=flat-square)](https://agentsonenterprise.com/)

</div>

---

## 🎯 What You'll Learn

This chapter shows you how to implement production-grade observability for your AI agent systems using .NET Aspire and OpenTelemetry.

### Topics Covered

- **Distributed Tracing** — End-to-end request tracing across agents
- **Metrics Collection** — Performance, usage, and business metrics
- **Logging Strategy** — Structured logging for agent systems
- **.NET Aspire** — Cloud-native orchestration and observability

---

## 📁 Code Examples

| Example | Description | Key Concepts |
|:---|:---|:---|
| [SupplyChain.AppHost](./SupplyChain.AppHost/) | Aspire orchestration | Service discovery, orchestration |
| [SupplyChain.ApiService](./SupplyChain.ApiService/) | Agent API with tracing | OpenTelemetry, metrics |
| [SupplyChain.Web](./SupplyChain.Web/) | Web frontend | End-to-end tracing |
| [SupplyChain.ServiceDefaults](./SupplyChain.ServiceDefaults/) | Shared observability config | Reusable patterns |

---

## 🚀 Quick Start

```bash
# Run the Aspire application
cd part-08-enterprise/22-observability/SupplyChain.AppHost
dotnet run

# Open the Aspire dashboard at https://localhost:15000
```

---

## 💡 Key Pattern: OpenTelemetry Integration

```csharp
// Configure OpenTelemetry in ServiceDefaults
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("AgentTracing"))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddMeter("AgentMetrics"));
```

---

## 📊 Observability Stack

```
┌─────────────────────────────────────────────┐
│            .NET Aspire Dashboard             │
├──────────────┬──────────────┬───────────────┤
│    Traces    │    Metrics   │     Logs      │
├──────────────┴──────────────┴───────────────┤
│              OpenTelemetry                   │
├─────────────────────────────────────────────┤
│     Agent Services (instrumented)            │
└─────────────────────────────────────────────┘
```

---

## 🔒 Enterprise Considerations

| Concern | Solution |
|:---|:---|
| **Performance** | Sampling strategies, async export |
| **Cost** | Selective tracing, metric aggregation |
| **Security** | PII filtering in traces |
| **Compliance** | Audit trail retention |

---

## 📖 Continue Reading

These examples demonstrate the patterns. The complete chapter includes:

- 📊 Custom metrics for AI agents
- 🔍 Distributed tracing best practices
- 🚨 Alerting and incident response
- 🏢 Enterprise observability strategy

<div align="center">

[![Get the Book](https://img.shields.io/badge/Get%20the%20Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

</div>

---

[← Previous: ADLC](../21-adlc/README.md) | [Next: Evaluation →](../23-evaluation/README.md)
