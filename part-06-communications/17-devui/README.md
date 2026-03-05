# Chapter 17: Developer UI

<div align="center">

*Building enterprise KYC services with developer-friendly interfaces*

[![Get the Full Book](https://img.shields.io/badge/📘_Full_Chapter_in_Book-agentsonenterprise.com-blue?style=flat-square)](https://agentsonenterprise.com/)

</div>

---

## 🎯 What You'll Learn

This chapter demonstrates building developer-friendly agent interfaces for enterprise services like KYC (Know Your Customer).

### Topics Covered

- **Developer Experience** — Building APIs that developers love
- **KYC Integration** — AI agents for compliance workflows
- **Testing Interfaces** — Tools for agent development and debugging
- **Documentation** — Auto-generated API documentation

---

## 📁 Code Examples

| Example | Description | Key Concepts |
|:---|:---|:---|
| [EnterpriseKycService](./EnterpriseKycService/) | KYC agent service | Compliance, API design, testing |

---

## 🚀 Quick Start

```bash
# Run the KYC service
cd part-06-communications/17-devui/EnterpriseKycService
dotnet run

# Access the API at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger
```

---

## 💡 Key Pattern: Developer-Friendly Agent API

```csharp
// Well-documented, testable agent endpoints
app.MapPost("/api/kyc/verify", async (KycRequest request) =>
{
    var result = await kycAgent.VerifyCustomerAsync(request);
    return Results.Ok(new KycResponse
    {
        Status = result.Status,
        RiskScore = result.RiskScore,
        Recommendations = result.Recommendations
    });
})
.WithName("VerifyCustomer")
.WithOpenApi();
```

---

## 🔒 Enterprise Considerations

| Concern | Solution |
|:---|:---|
| **Compliance** | Audit trails, data retention |
| **Testing** | Sandbox environments, mock data |
| **Versioning** | API versioning strategies |
| **Documentation** | OpenAPI/Swagger generation |

---

## 📖 Continue Reading

This example demonstrates the pattern. The complete chapter includes:

- 📝 API design best practices
- 🧪 Testing strategies for agent services
- 📚 Documentation automation
- 🏢 Enterprise API governance

<div align="center">

[![Get the Book](https://img.shields.io/badge/Get%20the%20Book-agentsonenterprise.com-blue?style=for-the-badge)](https://agentsonenterprise.com/)

</div>

---

[← Previous: AG-UI](../16-ag-ui/README.md) | [Next: Part VII - Advanced →](../../part-07-advanced/README.md)
