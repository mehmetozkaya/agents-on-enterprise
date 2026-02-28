using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

class BasicTextRAGExample
{
    // Define the raw enterprise data as a static field for the search adapter
    private static readonly string CorporatePolicy = @"
        [Effective Jan 2026] Contoso Corporate Remote Work Policy:
        - Tier 1 Employees: Must be in the office 3 days a week (Tuesday, Wednesday, Thursday).
        - Tier 2 Employees: Fully remote permitted.
        - Hardware Budget: $500 every two years for home office equipment.
        - Security: All remote work must be conducted over the corporate VPN using an issued device.
    ";

    static async Task Main()
    {
        string endpoint = "https://agents-on-foundry-resource.services.ai.azure.com/";
        string deploymentName = "gpt-5-mini";

        ChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
            .GetChatClient(deploymentName);

        // Configure the options for the TextSearchProvider
        TextSearchProviderOptions textSearchOptions = new()
        {
            SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
        };

        // Create the AI agent with the TextSearchProvider as the AI context provider
        AIAgent hrAgent = chatClient.AsAIAgent(new ChatClientAgentOptions()
        {
            Name = "HRAssistant",
            ChatOptions = new()
            {
                Instructions = "You are an HR policy assistant. Answer questions using the provided context and cite the source document when available."
            },
            AIContextProviders = [new TextSearchProvider(SearchAdapter, textSearchOptions)]
        });

        Console.WriteLine($"Agent '{hrAgent.Name}' is online with In-Memory RAG.\n");

        // The agent will autonomously use the TextSearchProvider to find relevant policy information
        string prompt = "I am a Tier 1 employee. What days do I need to be in the office, and how much can I expense for a new monitor?";
        Console.WriteLine($"User: {prompt}");

        AgentResponse response = await hrAgent.RunAsync(prompt);
        Console.WriteLine($"Agent: {response.Text}");
    }

    /// <summary>
    /// Mock search adapter that searches the corporate policy based on query keywords.
    /// In production, this would connect to Azure AI Search or another search technology.
    /// </summary>
    static Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAdapter(string query, CancellationToken cancellationToken)
    {
        List<TextSearchProvider.TextSearchResult> results = [];

        // Search for office/remote work related queries
        if (query.Contains("office", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("remote", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("tier", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("days", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new()
            {
                SourceName = "Contoso Corporate Remote Work Policy",
                SourceLink = "https://contoso.com/policies/remote-work",
                Text = "Tier 1 Employees: Must be in the office 3 days a week (Tuesday, Wednesday, Thursday). Tier 2 Employees: Fully remote permitted."
            });
        }

        // Search for hardware/budget related queries
        if (query.Contains("hardware", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("budget", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("expense", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("monitor", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("equipment", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new()
            {
                SourceName = "Contoso Corporate Remote Work Policy",
                SourceLink = "https://contoso.com/policies/remote-work#hardware",
                Text = "Hardware Budget: $500 every two years for home office equipment."
            });
        }

        // Search for security related queries
        if (query.Contains("security", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("vpn", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("device", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new()
            {
                SourceName = "Contoso Corporate Remote Work Policy",
                SourceLink = "https://contoso.com/policies/remote-work#security",
                Text = "Security: All remote work must be conducted over the corporate VPN using an issued device."
            });
        }

        // If no specific matches, return full policy
        if (results.Count == 0)
        {
            results.Add(new()
            {
                SourceName = "Contoso Corporate Remote Work Policy",
                SourceLink = "https://contoso.com/policies/remote-work",
                Text = CorporatePolicy
            });
        }

        return Task.FromResult<IEnumerable<TextSearchProvider.TextSearchResult>>(results);
    }
}