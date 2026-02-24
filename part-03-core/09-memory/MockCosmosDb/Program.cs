using System.Text.Json;
using Azure.Identity;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

// 1. Define the Enterprise Storage Contract
public interface ISessionRepository
{
    Task<string?> GetSessionJsonAsync(string sessionId);
    Task SaveSessionJsonAsync(string sessionId, string jsonPayload);
}

// (Mock implementation of a database like Cosmos DB or SQL Server)
public class MockCosmosDbRepository : ISessionRepository
{
    private readonly System.Collections.Generic.Dictionary<string, string> _datastore = new();

    public Task<string?> GetSessionJsonAsync(string sessionId) =>
        Task.FromResult(_datastore.TryGetValue(sessionId, out var json) ? json : null);

    public Task SaveSessionJsonAsync(string sessionId, string jsonPayload)
    {
        _datastore[sessionId] = jsonPayload;
        return Task.CompletedTask;
    }
}

class StatelessAgentService
{
    private readonly AIAgent _agent;
    private readonly ISessionRepository _repository;

    public StatelessAgentService(ISessionRepository repository)
    {
        _repository = repository;

        string endpoint = "https://agents-on-foundry-resource.services.ai.azure.com/";
        _agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
            .GetChatClient("gpt-5-mini")
            .AsAIAgent(
                name: "PersistentGuide",
                instructions: "You are a helpful assistant. You remember details across long periods of time."
                );
    }

    // 2. The Stateless Execution Method (e.g., called by an ASP.NET Controller)
    public async Task<string> HandleUserMessageAsync(string sessionId, string userMessage)
    {
        AgentSession session;

        // Step A: Attempt to retrieve historical state from the database
        string? savedSessionJson = await _repository.GetSessionJsonAsync(sessionId);

        if (!string.IsNullOrEmpty(savedSessionJson))
        {
            // Parse the database string back into a JsonElement
            using JsonDocument doc = JsonDocument.Parse(savedSessionJson);

            // Step B: Deserialize the session, restoring the agent's memory
            session = await _agent.DeserializeSessionAsync(doc.RootElement);
            Console.WriteLine($"[SYSTEM LOG] Successfully restored session {sessionId} from database.");
        }
        else
        {
            // Step C: Fallback - Create a brand new session if no history exists
            session = await _agent.CreateSessionAsync();
            Console.WriteLine($"[SYSTEM LOG] Created new session for {sessionId}.");
        }

        // Step D: Execute the agent with the loaded session
        AgentResponse response = await _agent.RunAsync(userMessage, session);

        // Step E: Serialize the newly updated session state
        JsonElement updatedSessionElement = await _agent.SerializeSessionAsync(session);
        string updatedJsonString = JsonSerializer.Serialize(updatedSessionElement);

        // Step F: Persist the updated state back to the database
        await _repository.SaveSessionJsonAsync(sessionId, updatedJsonString);

        return response.Text;
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        // Setup our mock database and agent service
        var repository = new MockCosmosDbRepository();
        var agentService = new StatelessAgentService(repository);

        string userId = "user-778899";

        Console.WriteLine("--- Monday Morning ---");
        string response1 = await agentService.HandleUserMessageAsync(userId, "Hi, I am planning a trip to Tokyo next month.");
        Console.WriteLine($"Agent: {response1}\n");

        // The application could completely shut down or restart here.
        // The memory is safely stored in the repository.

        Console.WriteLine("--- Friday Afternoon (Simulating a new server request) ---");
        string response2 = await agentService.HandleUserMessageAsync(userId, "Do you remember where I said I was traveling?");
        Console.WriteLine($"Agent: {response2}\n");
    }
}