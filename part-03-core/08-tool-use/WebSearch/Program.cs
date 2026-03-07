using Azure.Identity;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

// Define the variables we extracted from Microsoft Foundry
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

// Initialize the Agent with live internet access
AIAgent researchAgent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(
        name: "MarketResearcher",
        instructions: "You are a market researcher. Always verify current events using the Web Search tool before providing an answer. Cite your sources."
        // tools: [new WebSearchToolDefinition()]
    );
// TODO: The WebSearchToolDefinition is not working yet, but once it is, this will allow the agent to autonomously perform live web searches to retrieve up-to-date information from the internet. The agent can then synthesize answers based on the latest news and data while providing citations to the sources it used, ensuring transparency and reliability in its responses.

Console.WriteLine($"Agent '{researchAgent.Name}' is online with live internet access.\n");

string prompt = "What were the major tech news announcements made yesterday regarding artificial intelligence?";
Console.WriteLine($"User: {prompt}");

// The agent autonomously queries the web, reads the top articles, and summarizes the live events.
AgentResponse response = await researchAgent.RunAsync(prompt);
Console.WriteLine($"Agent: {response.Text}");
