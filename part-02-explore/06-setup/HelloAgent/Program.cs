using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI.Chat;

// 1. Define the variables we extracted from Microsoft Foundry
string endpoint = "https://agents-on-foundry-resource.services.ai.azure.com/";
string model = "gpt-5-mini";

// 2. Create the Agent using MAF
AIAgent agent = new AzureOpenAIClient(
        new Uri(endpoint),
        new AzureCliCredential())
    .GetChatClient(model)
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.");

// 3. Invoke the Agent
Console.WriteLine(await agent.RunAsync("What is the largest city in France?"));
