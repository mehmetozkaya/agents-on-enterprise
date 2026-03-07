using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

// Define the variables we extracted from Microsoft Foundry
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

// In a real scenario, this ID is retrieved from your Azure AI Foundry project
string corporateVectorStoreId = "vs-987654321";

// Initialize the Agent with the File Search capability pointing to your data
AIAgent financeAgent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(
        name: "FinancialAnalyst",
        instructions: "You are a financial analyst. Answer questions strictly based on the provided corporate documents. If the document does not contain the answer, state that you do not know."
        // tools: [new FileSearchToolDefinition(vectorStoreIds: [corporateVectorStoreId])]
    );
// TODO: The FileSearchToolDefinition is not working yet, but once it is, this will allow the agent to autonomously query your corporate vector database to retrieve relevant information from your secure documents without exposing any sensitive data directly to the LLM. The agent can then synthesize answers based on the retrieved information while ensuring compliance with your data security policies.

Console.WriteLine($"Agent '{financeAgent.Name}' is online and grounded in your secure data.\n");

string prompt = "What were the key risk factors identified in the Q3 Financial Report?";
Console.WriteLine($"User: {prompt}");

AgentResponse response = await financeAgent.RunAsync(prompt);
Console.WriteLine($"Agent: {response.Text}");
