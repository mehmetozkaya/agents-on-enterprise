using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.Text;
using System.Text.Json;

// 1. Define the strongly-typed state object
internal sealed class EmployeeProfile
{
    public string? EmployeeName { get; set; }
    public string? Department { get; set; }
}

// 2. Define the Context Provider
internal sealed class EmployeeProfileProvider : AIContextProvider
{
    // Securely binds our custom state to the current AgentSession
    private readonly ProviderSessionState<EmployeeProfile> _sessionState;
    // A secondary client used for background data extraction
    private readonly IChatClient _chatClient;

    public EmployeeProfileProvider(IChatClient chatClient) : base(null, null)
    {
        _sessionState = new ProviderSessionState<EmployeeProfile>(
            _ => new EmployeeProfile(),
            this.GetType().Name);
        _chatClient = chatClient;
    }

    public override IReadOnlyList<string> StateKeys => [_sessionState.StateKey];

    public EmployeeProfile GetProfile(AgentSession session) => _sessionState.GetOrInitializeState(session);

    // Phase 1: Pre-Invocation (Injecting Context)
    protected override ValueTask<AIContext> ProvideAIContextAsync(InvokingContext context, CancellationToken cancellationToken = default)
    {
        var profile = _sessionState.GetOrInitializeState(context.Session);
        StringBuilder instructions = new();

        // Dynamically build instructions based on the current known state
        instructions
            .AppendLine(profile.EmployeeName is null ?
                "Ask the user for their name and politely decline to answer corporate questions until they provide it." :
                $"The user's name is {profile.EmployeeName}.")
            .AppendLine(profile.Department is null ?
                "Ask the user for their department (e.g., HR, IT, Finance) and politely decline to answer corporate questions until they provide it." :
                $"The user's department is {profile.Department}. Tailor your answers to this department.");

        return new ValueTask<AIContext>(new AIContext
        {
            Instructions = instructions.ToString()
        });
    }

    // Phase 2: Post-Invocation (Extracting & Storing State)
    protected override async ValueTask StoreAIContextAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        var profile = _sessionState.GetOrInitializeState(context.Session);

        // If we are missing data, run a lightweight background LLM extraction on the user's messages
        if ((profile.EmployeeName is null || profile.Department is null) && context.RequestMessages.Any(x => x.Role == ChatRole.User))
        {
            var result = await _chatClient.GetResponseAsync<EmployeeProfile>(
                context.RequestMessages,
                new ChatOptions()
                {
                    Instructions = "Extract the user's name and corporate department from the message if present. If not present, return nulls."
                },
                cancellationToken: cancellationToken);

            // Update state with extracted data
            profile.EmployeeName ??= result.Result?.EmployeeName;
            profile.Department ??= result.Result?.Department;
        }

        // Save the updated state back to the session
        _sessionState.SaveState(context.Session, profile);
    }
}

class Program
{
    static async Task Main(string[] args)
    {        
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

        ChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
            .GetChatClient(deploymentName);

        // 3. Initialize the Agent with our custom stateful provider
        AIAgent agent = chatClient.AsAIAgent(new ChatClientAgentOptions()
        {
            Name = "CorporateGuide",
            ChatOptions = new() { Instructions = "You are a friendly internal corporate assistant." },
            AIContextProviders = [new EmployeeProfileProvider(chatClient.AsIChatClient())]
        });

        // 4. Create a new session
        AgentSession session = await agent.CreateSessionAsync();

        Console.WriteLine("--- Starting Fresh Session ---");        
        Console.WriteLine(await agent.RunAsync("What is the company policy on remote work?", session));               
        Console.WriteLine(await agent.RunAsync("My name is Mehmet and I work in the IT Department.", session));

        // 5. Serialize the session (This automatically captures the extracted EmployeeProfile)
        JsonElement serializedSession = await agent.SerializeSessionAsync(session);

        Console.WriteLine("\n--- Simulating a New Day (Deserializing Session) ---");

        // 6. Deserialize and resume
        var resumedSession = await agent.DeserializeSessionAsync(serializedSession);

        Console.WriteLine(await agent.RunAsync("Can you remind me of my department?", resumedSession));

        // 7. Accessing the strongly-typed memory directly from code
        var profileProvider = agent.GetService<EmployeeProfileProvider>();
        var profile = profileProvider?.GetProfile(resumedSession);

        Console.WriteLine("\n[SYSTEM DIAGNOSTICS] Explicitly reading memory component:");
        Console.WriteLine($"Extracted Name: {profile?.EmployeeName}");
        Console.WriteLine($"Extracted Department: {profile?.Department}");
    }
}