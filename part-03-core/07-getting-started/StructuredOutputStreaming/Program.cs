using System.Text.Json.Serialization;
using System.Text.Json;
using Azure.Identity;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;


// 1. Define the variables we extracted from Microsoft Foundry
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

// 2. Initialize the Agent from AzureOpenAIClient via IChatClient
AIAgent meetingAgent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(new ChatClientAgentOptions()
    {
        Name = "MeetingAnalyst",
        Description = "You are an AI analyst. Extract the topic, action items, and overall sentiment from the provided transcript.",
        ChatOptions = new() { ResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat.ForJsonSchema<MeetingAnalysis>() }
    });

// 3. Execute the Agent with RunAsync<T> for strongly-typed structured output
string transcript = "We discussed the Q4 marketing push. Sarah needs to finalize the budget by Tuesday. John will contact the ad agency. Overall, everyone felt very optimistic about the campaign.";
Console.WriteLine($"Analyzing Transcript:\n{transcript}\n");

IAsyncEnumerable<AgentResponseUpdate> updates = meetingAgent.RunStreamingAsync(transcript);

AgentResponse response = await updates.ToAgentResponseAsync();

// 4. Access the strongly-typed Result directly (no manual deserialization needed)
MeetingAnalysis analysis = JsonSerializer.Deserialize<MeetingAnalysis>(response.Text)!;

// 5. Utilize deterministic C# objects
if (analysis != null)
{
    Console.WriteLine($"Full Analysis: {analysis}\n");
    Console.WriteLine($"Topic: {analysis.Topic}");
    Console.WriteLine($"Sentiment: {analysis.Sentiment}");
    Console.WriteLine($"Action Items Count: {analysis.ActionItems.Length}");
    Console.WriteLine($"Action Items:");
    foreach (var item in analysis.ActionItems)
    {
        Console.WriteLine($"- {item}");
    }
}

// Data Contract
public record MeetingAnalysis(
    [property: JsonPropertyName("topic")] string Topic,
    [property: JsonPropertyName("actionItems")] string[] ActionItems,
    [property: JsonPropertyName("sentiment")] string Sentiment
);