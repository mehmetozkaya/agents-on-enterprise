using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Register the foundational Chat Client
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

IChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

builder.Services.AddSingleton(chatClient);

// Register the Specialized Enterprise Agent
var complianceAgent = builder.AddAIAgent(
    name: "compliance",
    instructions: "You are a strict enterprise compliance auditor. Review the provided text for GDPR violations. Be concise and authoritative."
);


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Expose the agent via the A2A protocol
// We also define an AgentCard, which acts as the agent's public business card for discovery
app.MapA2A(complianceAgent, path: "/a2a/compliance", agentCard: new()
{
    Name = "GDPR Compliance Agent",
    Description = "An autonomous auditor that reviews text for European data privacy violations.",
    Version = "1.0.0"
});

app.Run();
