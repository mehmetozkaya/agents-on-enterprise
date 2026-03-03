using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DevUI internal services to the Dependency Injection container
builder.Services.AddDevUI();

// 2. Register services for OpenAI responses and conversations 
// DevUI requires these standard endpoint abstractions to interact with your agents
builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

// 3. Initialize the LLM Chat Client
string endpoint = "https://agents-on-foundry-resource.services.ai.azure.com/";
string deploymentName = "gpt-5-mini";

var chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

// 4. Define the Enterprise Agent and its internal tools
var kycTool = AIFunctionFactory.Create(
    (string documentId, string documentType) =>
    {
        // Mock compliance database check
        if (documentId.StartsWith("INV")) return "REJECTED: Invalid Document Format.";
        return "APPROVED: Document passes cryptographic verification.";
    },
    name: "VerifyIdentityDocument",
    description: "Validates a government-issued ID against the compliance database."
);

AIAgent kycAgent = chatClient.AsAIAgent(
    name: "KycVerificationAgent",
    instructions: "You are a Level 1 KYC Compliance Agent. You must verify user documents using your verification tool before welcoming them to the platform.",
    tools: [kycTool]
);

// Explicitly register the agent so the DevUI dashboard can discover it
builder.Services.AddSingleton(kycAgent);

var app = builder.Build();

// 5. Map endpoints for OpenAI responses and conversations
// This allows the DevUI frontend to natively query the backend agents using standard payload structures
app.MapOpenAIResponses();
app.MapOpenAIConversations();

// 6. SECURITY BOUNDARY: Only map the DevUI endpoints in local development environments
if (app.Environment.IsDevelopment())
{
    // This exposes the visual dashboard and the WebSocket/SSE tracing connections
    app.MapDevUI();
}

app.Run("http://localhost:5000");