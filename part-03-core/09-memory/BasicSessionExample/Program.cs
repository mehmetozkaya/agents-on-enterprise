using Azure.Identity;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

string endpoint = "https://agents-on-foundry-resource.services.ai.azure.com/";
string deploymentName = "gpt-5-mini";

// 1. Initialize the Agent
AIAgent supportAgent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(
        name: "ITSupport",
        instructions: "You are an IT support assistant. Keep responses brief and remember the user's provided details."
    );

// 2. Create the Session (The memory container)
AgentSession session = await supportAgent.CreateSessionAsync();

// 3. Turn One: Providing Context
string prompt1 = "I am currently troubleshooting database server DB-PROD-04.";
Console.WriteLine($"User: {prompt1}");

// We pass the session object into the RunAsync method
AgentResponse firstResponse = await supportAgent.RunAsync(prompt1, session);
Console.WriteLine($"Agent: {firstResponse.Text}\n");

// 4. Turn Two: Testing Recall
string prompt2 = "Can you remind me which server I am looking at?";
Console.WriteLine($"User: {prompt2}");

// We pass the exact same session object again
AgentResponse secondResponse = await supportAgent.RunAsync(prompt2, session);
Console.WriteLine($"Agent: {secondResponse.Text}");
