using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI.Chat;


Console.WriteLine("--- Initializing Local DevOps Swarm ---");

// 1. Establish the Local MCP Connection
// The framework will execute 'npx' to launch the GitHub MCP server locally.
// NOTE: This requires Node.js to be installed on the host machine and a GITHUB_PERSONAL_ACCESS_TOKEN environment variable.
await using var mcpClient = await McpClient.CreateAsync(new StdioClientTransport(new()
{
    Name = "MCPServer",
    Command = "npx",
    Arguments = ["-y", "--verbose", "@modelcontextprotocol/server-github"]
}));

// 2. Capability Discovery
// Ask the MCP server what tools it exposes (e.g., search_repositories, get_commit, list_issues)
var mcpTools = await mcpClient.ListToolsAsync().ConfigureAwait(false);
Console.WriteLine($"[System] Discovered {mcpTools.Count()} tools from the local GitHub MCP server.");

// 3. Initialize the Enterprise Agent
// We cast the discovered MCP tools into native AITool definitions and inject them into the agent.
string endpoint = "https://agents-on-foundry-resource.services.ai.azure.com/";
string deploymentName = "gpt-5-mini";

AIAgent releaseAgent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
     .GetChatClient(deploymentName)
     .AsAIAgent(
         name: "ReleaseManager",
         instructions: "You are a DevOps Release Manager. You must only answer questions related to GitHub repositories. Use your tools to fetch commit history and summarize it into professional release notes.",
         tools: [.. mcpTools.Cast<AITool>()]
     );

// 4. Execution
string prompt = "Fetch the last 3 commits from the microsoft/agent-framework repository and summarize them for our V1.2 release notes.";
Console.WriteLine($"\nUser: {prompt}\n");
Console.WriteLine("[System] Agent is autonomously querying GitHub...");

AgentResponse response = await releaseAgent.RunAsync(prompt);
Console.WriteLine($"\nRelease Manager:\n{response.Text}");