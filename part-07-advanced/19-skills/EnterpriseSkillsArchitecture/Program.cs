using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

string endpoint = "https://agents-on-foundry-resource.services.ai.azure.com/";
string deploymentName = "gpt-5-mini";

Console.WriteLine("--- Initializing Skill-Based Agent ---");

// 1. Define the Skills Directory
// The provider will recursively scan this path for valid SKILL.md files.
string skillsDirectory = Path.Combine(AppContext.BaseDirectory, "skills");


// TODO: FileAgentSkillsProvider is not yet implemented. This is a placeholder for when it is available in the SDK.
//var skillsProvider = new FileAgentSkillsProvider(skillPath: skillsDirectory);

//// 2. Initialize the Agent Architecture
//// Notice how lean the base Instructions are. The deep knowledge is deferred to the skills.
//AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
//    .GetChatClient(deploymentName)
//    .AsAIAgent(new ChatClientAgentOptions
//    {
//        Name = "EnterpriseOperationsAgent",
//        ChatOptions = new()
//        {
//            Instructions = "You are a helpful enterprise operations assistant. Use your available skills to answer domain-specific questions.",
//        },
//        // 3. Inject the Context Provider
//        // This equips the agent with the 'load_skill' and 'read_skill_resource' tools natively
//        AIContextProviders = [skillsProvider],
//    });

//// 4. Invoking the Agent
//string userPrompt = "Are tips reimbursable? I left a 25% tip on a taxi ride to the airport.";
//Console.WriteLine($"User: {userPrompt}");
//Console.WriteLine("[System] Agent is analyzing intent and lazy-loading relevant skills...");

//// The agent automatically discovers the expense-report skill, loads it, 
//// reads the associated FAQ resource, and synthesizes the final answer.
//AgentResponse response = await agent.RunAsync(userPrompt);

//Console.WriteLine($"\nAgent: {response.Text}");
