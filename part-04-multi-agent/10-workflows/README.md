# Chapter 10: Multi-Agent Workflows

<div align="center">

*Agentic workflow patterns, checkpoints, and human-in-the-loop orchestration*

[![Get the Full Book](https://img.shields.io/badge/📘_Full_Chapter_in_Book-agentsonenterprise.com-blue?style=flat-square)](https://agentsonenterprise.com/)

</div>

---

## 🎯 What You'll Learn

This chapter covers the patterns and practices for orchestrating multiple agents to solve complex enterprise problems.

### Topics Covered

- **Workflow Patterns** — Sequential, parallel, and hierarchical orchestration
- **Agent Handoffs** — Passing context between specialized agents
- **Checkpoints** — Saving and resuming long-running workflows
- **Human-in-the-Loop** — Approval points and manual intervention

---

## 📁 Code Examples

| Example | Description | Key Concepts |
|:---|:---|:---|
| [FirstWorkflow](./FirstWorkflow/) | Basic workflow setup | Workflow initialization |
| [AgentsWorkflow](./AgentsWorkflow/) | Multi-agent orchestration | Agent coordination, handoffs |
| [AgenticWorkflowPatterns](./AgenticWorkflowPatterns/) | Enterprise workflow patterns | Production patterns |
| [CheckpointHumanInTheLoop](./CheckpointHumanInTheLoop/) | Checkpoints & approvals | State persistence, approvals |

---

## 🚀 Quick Start

```bash
# Set environment variables (PowerShell)
$env:AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"  # Replace with your Azure OpenAI resource endpoint
$env:AZURE_OPENAI_DEPLOYMENT_NAME="gpt-5-mini"  # Optional, defaults to gpt-5-mini

# Run the basic workflow example
cd part-04-multi-agent/10-workflows/FirstWorkflow
dotnet run

# Try human-in-the-loop checkpoints
cd ../CheckpointHumanInTheLoop
dotnet run
```

---

## 🏗️ Workflow Patterns

### Sequential Workflow
```
Agent A ──▶ Agent B ──▶ Agent C ──▶ Result
```

### Parallel Workflow
```
        ┌── Agent A ──┐
Input ──┤── Agent B ──├── Aggregate ── Result
        └── Agent C ──┘
```

### Hierarchical Workflow
```
            ┌── Worker A
Supervisor ──┤── Worker B
            └── Worker C
```

---

## 🔒 Enterprise Considerations

| Concern | Solution |
|:---|:---|
| **Reliability** | Checkpoints for workflow recovery |
| **Auditability** | Full trace of agent decisions |
| **Control** | Human approval for critical actions |
| **Scalability** | Distributed workflow execution |

---

## 📖 Continue Reading

These examples demonstrate the patterns. The complete chapter includes:

- 🏗️ Advanced orchestration architectures
- 🔄 Workflow recovery and error handling
- 📊 Performance optimization for multi-agent systems
- 🏢 Enterprise deployment patterns

<div align="center">

[![Get the Book](https://img.shields.io/badge/Get%20the%20Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

</div>

---

[← Back to Part IV](../README.md) | [Next: Part V - Agentic RAG →](../../part-05-agentic-rag/README.md)
