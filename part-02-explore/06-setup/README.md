# Chapter 06: Environment Setup

<div align="center">

*Setting up your .NET development environment for building AI agents*

[![Get the Full Book](https://img.shields.io/badge/📘_Full_Chapter_in_Book-agentsonenterprise.com-blue?style=flat-square)](https://agentsonenterprise.com/)

</div>

---

## 🎯 What You'll Learn

This chapter gets you from zero to running your first AI agent with a properly configured enterprise development environment.

### Topics Covered

- **Prerequisites** — .NET SDK, Azure CLI, IDE setup
- **Azure AI Foundry Configuration** — Setting up your cloud resources
- **Authentication** — Secure keyless authentication with Azure CLI
- **First Agent** — Building and running your "Hello Agent" application

---

## 📁 Code Examples

| Example | Description | Key Concepts |
|:---|:---|:---|
| [HelloAgent](./HelloAgent/) | Your first AI agent | MAF basics, Azure OpenAI, authentication |

---

## 🚀 Quick Start

```bash
# Navigate to the example
cd part-02-explore/06-setup/HelloAgent

# Set environment variables (PowerShell)
$env:AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"  # Replace with your Azure OpenAI resource endpoint
$env:AZURE_OPENAI_DEPLOYMENT_NAME="gpt-5-mini"  # Optional, defaults to gpt-5-mini

# Run the agent
dotnet run
```

---

# Setting Up Your Local Development Environment

This is the section where we get our hands dirty and set up a local development environment to interact with the Azure AI Foundry resource we created in the previous chapter. We will be using **.NET 8** for this setup, but the principles apply across languages and frameworks.

## Development Code: The Minimalist Enterprise Approach

Open your `Program.cs` file and replace its contents with the following **minimalist code**. This utilizes `AzureCliCredential()` to authenticate securely without hardcoding API keys.

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

// 1. Define the variables we extracted from Microsoft Foundry
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

// 2. Create the Agent using MAF
AIAgent agent = new AzureOpenAIClient(
        new Uri(endpoint),        
        new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.");

// 3. Invoke the Agent
Console.WriteLine(await agent.RunAsync("What is the largest city in France?"));
```

This compact syntax demonstrates the power of the **.NET AI ecosystem's abstraction layers**:

* **Step 1:** We define the routing variables. The endpoint tells the SDK which Azure resource to hit, and the model maps to the exact Deployment Name you created in Foundry.
* **Step 2:** We chain the initialization. We instantiate the `AzureOpenAIClient` using `AzureCliCredential()`, which silently requests a **secure token from Microsoft Entra ID** using your local `az login` session. We immediately retrieve the chat client using `.GetChatClient(model)`, and then utilize the `.AsAIAgent()` extension method. This final method casts the raw client into an autonomous AIAgent object provided by the **Microsoft Agent Framework**, injecting its core persona via the instructions parameter.
* **Step 3:** We invoke the agent asynchronously using `.RunAsync()` and print the response directly to the console.

Run the application using `dotnet run`. The application will **authenticate, connect to the cloud, and print the agent's response**:

```bash
Paris - it's France's largest city (city proper about 2.1 million people; the Paris metropolitan area around 12 million).
```

## Alternative: Using API Key Authentication
While **keyless authentication** is the enterprise standard, there are scenarios (such as specific local testing constraints or integrating with legacy CI/CD pipelines) where you may need to fallback to a **traditional API Key**.

You can retrieve your **API Key** from the **Models + endpoints section** in your Azure AI Foundry project.

Here is how the identical agent is instantiated using an `ApiKeyCredential`:

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

// 1. Define the variables we extracted from Microsoft Foundry
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

// 2. Create the Agent using MAF
AIAgent agent = new AzureOpenAIClient(
        new Uri(endpoint),
        new ApiKeyCredential(apiKey))        
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.");

// 3. Invoke the Agent
Console.WriteLine(await agent.RunAsync("What is the largest city in France?"));

```

The orchestration logic remains **entirely untouched**. The only modification occurs at the initialization layer in **Step 2**, where we swap `new AzureCliCredential()` for `new ApiKeyCredential(apiKey)`. The `.AsAIAgent()` extension method ensures that the agent itself remains ignorant of how the connection was authenticated.

::: important
**Important Architecture Rule**

Throughout the remainder of this book, we will exclusively use `AzureCliCredential()` alongside the `az login` command. **Eliminating hard-coded API keys** from your repositories is a **critical best practice** for **enterprise security**.
:::