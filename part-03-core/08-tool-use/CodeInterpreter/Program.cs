using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;


// Define the variables we extracted from Microsoft Foundry
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

// Initialize the Agent and inject the native Code Interpreter tool
AIAgent mathAgent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(
        name: "DataAnalyst",
        instructions: "You are a data analyst. You must write and execute Python code to answer complex math and data questions. Never guess the answer."
        // tools: [new CodeInterpreterToolDefinition()]
    );
// TODO: The CodeInterpreterToolDefinition is not working yet, but once it is, this will allow the agent to write and execute Python code in a secure sandbox environment that prevents any malicious activity while still enabling powerful computational capabilities.

Console.WriteLine($"Agent '{mathAgent.Name}' is online with a Python sandbox.\n");

// The agent will autonomously write a script to solve this, run it, and return the result.
string prompt = "Calculate the 50th Fibonacci number and determine if it is a prime number.";
Console.WriteLine($"User: {prompt}");

AgentResponse response = await mathAgent.RunAsync(prompt);
Console.WriteLine($"Agent: {response.Text}");
