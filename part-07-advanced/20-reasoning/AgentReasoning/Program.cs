#pragma warning disable OPENAI001 // Experimental API

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

Console.WriteLine("=== Agent Reasoning Demo: Comparing Reasoning Effort Levels ===\n");

// Initialize the Chat Client
IChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

// Single enterprise task to compare reasoning efforts
string architectureTask = """
    Analyze failure modes: Service A (Orders) calls Service B (Payment) via gRPC, 
    which publishes to Kafka for Service C (Inventory). Propose a Saga pattern fix.
    """;

// Compare different reasoning effort levels with same prompt
var effortLevels = new[] 
{ 
    ChatReasoningEffortLevel.Minimal, 
    ChatReasoningEffortLevel.Low, 
    ChatReasoningEffortLevel.Medium 
};

foreach (var effort in effortLevels)
{
    Console.WriteLine($"--- Architecture Review Agent (Effort: {effort}) ---");
    Console.WriteLine($"Task: {architectureTask}\n");

    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
    {
        Name = "ArchitectureReviewAgent",
        ChatOptions = new ChatOptions
        {
            RawRepresentationFactory = _ => new ChatCompletionOptions
            {
                ReasoningEffortLevel = effort
            }
        }
    });

    var response = await agent.RunAsync(architectureTask);

    // Display reasoning content (gray)
    foreach (Microsoft.Extensions.AI.ChatMessage message in response.Messages)
    {        
        foreach (AIContent content in message.Contents)
        {
            if (content is TextReasoningContent textReasoningContent)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"[Reasoning]: {textReasoningContent.Text}");
                Console.ResetColor();
            }
        }
    }

    // Get raw ChatResponse for token usage
    var chatResponse = response.RawRepresentation as ChatResponse;

    // Display response
    Console.WriteLine($"\nResponse:\n{response.Text}\n");

    // Display token usage (green)
    if (chatResponse?.Usage is not null)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[Tokens] Input: {chatResponse.Usage.InputTokenCount} | Output: {chatResponse.Usage.OutputTokenCount} | Reasoning: {chatResponse.Usage.ReasoningTokenCount ?? 0}");
        Console.ResetColor();
    }

    Console.WriteLine(new string('-', 80) + "\n");
}

Console.WriteLine("=== Demo Complete ===");

#pragma warning restore OPENAI001