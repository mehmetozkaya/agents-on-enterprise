# Chapter 15: Model Context Protocol (MCP)

<div align="center">

*Implementing MCP with GitHub integration and governance examples*

[![Get the Full Book](https://img.shields.io/badge/📘_Full_Chapter_in_Book-agentsonenterprise.com-blue?style=flat-square)](https://agentsonenterprise.com/)

</div>

---

## 🎯 What You'll Learn

This chapter covers the Model Context Protocol (MCP)—a standard for connecting AI models to external tools and data sources.

### Topics Covered

- **MCP Fundamentals** — Understanding the protocol and its benefits
- **MCP Servers** — Building tool providers for AI agents
- **MCP Clients** — Consuming MCP services in your agents
- **Enterprise Governance** — Controlling tool access and usage

---

## 📁 Code Examples

| Example | Description | Key Concepts |
|:---|:---|:---|
| [LocalGitHubMcpExample](./LocalGitHubMcpExample/) | GitHub integration via MCP | Local MCP server, Git operations |
| [HostedMcpGovernanceExample](./HostedMcpGovernanceExample/) | Enterprise governance patterns | Policy enforcement, access control |

---

## 🚀 Quick Start

```bash
# Run the GitHub MCP example
cd part-06-communications/15-mcp/LocalGitHubMcpExample
dotnet run

# Try the governance example
cd ../HostedMcpGovernanceExample
dotnet run
```

---

## 💡 Key Pattern: MCP Server

```csharp
// Define an MCP tool
[McpTool("search_code", "Search code in repositories")]
public async Task<SearchResult> SearchCode(
    [McpParameter("query")] string query,
    [McpParameter("repo")] string repository)
{
    // Implementation
    return await gitHubClient.SearchCodeAsync(query, repository);
}

// Register MCP server
builder.Services.AddMcpServer()
    .WithTools<GitHubTools>();
```

---

## 🔒 Enterprise Considerations

| Concern | Solution |
|:---|:---|
| **Access Control** | Role-based tool permissions |
| **Audit** | Log all tool invocations |
| **Rate Limiting** | Prevent API abuse |
| **Governance** | Central policy management |

---

## 📖 Continue Reading

These examples demonstrate the patterns. The complete chapter includes:

- 📜 Complete MCP specification walkthrough
- 🔧 Advanced tool development patterns
- 🔐 Security and authentication
- 🏢 Enterprise MCP deployment

<div align="center">

[![Get the Book](https://img.shields.io/badge/Get%20the%20Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

</div>

---

[← Previous: A2A](../14-a2a/README.md) | [Next: AG-UI →](../16-ag-ui/README.md)
