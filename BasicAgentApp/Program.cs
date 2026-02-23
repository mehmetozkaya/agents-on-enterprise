using Azure.Identity;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;


// 1. Establish the connection parameters
string endpoint = "https://agents-on-foundry-resource.services.ai.azure.com/";
string model = "gpt-5-mini";

// 2. Instantiate the universal chat client
IChatClient chatClient = new AzureOpenAIClient(
        new Uri(endpoint),
        new AzureCliCredential())
    .GetChatClient(model)
    .AsIChatClient();

// 3. Define the Agent's Anatomy
AIAgent supportAgent = chatClient.AsAIAgent(
    name: "NetworkSupport",
    instructions: "You are a Tier 1 IT Support Agent. Your answers must be concise, professional, and limited strictly to troubleshooting network and VPN connectivity."
);

Console.WriteLine($"Agent '{supportAgent.Name}' is online.\n");

// 4. Execute the Agent
string userIssue = "I am getting a DNS resolution error when connecting to the corporate VPN from a coffee shop.";
Console.WriteLine($"User: {userIssue}\n");

AgentResponse response = await supportAgent.RunAsync(userIssue);
Console.WriteLine($"Agent: {response.Text}");