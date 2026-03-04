using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


string endpoint = "https://agents-on-foundry-resource.services.ai.azure.com/";

// Initialize the Chat Client
var chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient("gpt-5-mini")
    .AsIChatClient();

// Define an enterprise tool (This execution will be automatically traced!)
var checkInventoryTool = AIFunctionFactory.Create(
    async (string productId) =>
    {
        // Simulate a slow database call
        await Task.Delay(100);
        return $"Product {productId} has 142 units in stock.";
    },
    name: "CheckInventory",
    description: "Queries the warehouse database for real-time stock levels."
);

// Architect the Agent
AIAgent inventoryAgent = chatClient.AsAIAgent(
    name: "SupplyChainAgent",
    instructions: "You are a supply chain assistant. Always check inventory before confirming an order.",
    tools: [checkInventoryTool]
);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/api/chat", async ([FromBody] string prompt) =>
{
    // The entire RunAsync loop is wrapped in a distributed trace
    AgentResponse response = await inventoryAgent.RunAsync(prompt);
    return response.Text;
});

app.MapDefaultEndpoints();

app.Run();
