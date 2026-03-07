using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

// 1. Register AG-UI services
builder.Services.AddHttpClient().AddLogging();
builder.Services.AddAGUI();

var app = builder.Build();

// 2. Initialize the LLM Chat Client and Define the Backend Agent
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

AIAgent enterpriseAgent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
     .GetChatClient(deploymentName)
     .AsAIAgent(
         name: "EnterpriseSupportAgent",
         instructions: "You are a helpful enterprise support agent."
     );

// 3. Expose the Agent via the AG-UI Protocol
// This single extension method automatically wires up HTTP POST processing and SSE streaming
app.MapAGUI("/agui/support", enterpriseAgent);

app.Run("http://localhost:5000");
