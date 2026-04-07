using AgenticWorkflowPatterns;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

class Program
{
    static async Task Main(string[] args)
    {        
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

        IChatClient client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
            .GetChatClient(deploymentName)
            .AsIChatClient();

        Console.WriteLine("Select Enterprise Topology:");
        Console.WriteLine("1. Sequential (Localization Pipeline)");
        Console.WriteLine("2. Concurrent (Parallel Analysis)");
        Console.WriteLine("3. Handoff (Triage & Support Routing)");
        Console.WriteLine("4. Group Chat (Crisis Management)");
        Console.Write("Choice: ");

        switch (Console.ReadLine())
        {
            case "1":
                // --- SEQUENTIAL PIPELINE ---
                // Data flows strictly from French -> Spanish -> English
                var sequentialWorkflow = AgentWorkflowBuilder.BuildSequential(
                    from lang in (string[])["French", "Spanish", "English"]
                    select GetTranslationAgent(lang, client)
                );

                await EnterpriseOrchestrator.RunWorkflowAsync(
                    sequentialWorkflow,
                    [new ChatMessage(ChatRole.User, "The new enterprise software update will be deployed at midnight.")]
                );
                break;

            case "2":
                // --- CONCURRENT PIPELINE ---
                // All three agents process the identical payload simultaneously to reduce latency
                var concurrentWorkflow = AgentWorkflowBuilder.BuildConcurrent(
                    from lang in (string[])["French", "Spanish", "English"]
                    select GetTranslationAgent(lang, client)
                );

                await EnterpriseOrchestrator.RunWorkflowAsync(
                    concurrentWorkflow,
                    [new ChatMessage(ChatRole.User, "The new enterprise software update will be deployed at midnight.")]
                );
                break;

            case "3":
                // --- HANDOFF ROUTING ---
                // Triage analyzes the user intent and delegates execution to the correct specialist
                ChatClientAgent networkAdmin = new(client,
                    "You resolve network connectivity and DNS issues. Explain technical steps clearly.",
                    "Network_Admin", "Specialist for networking");

                ChatClientAgent billingSupport = new(client,
                    "You handle enterprise invoice and licensing queries.",
                    "Billing_Support", "Specialist for licensing and billing");

                ChatClientAgent triageRouter = new(client,
                    "Determine if the user needs Network or Billing support. ALWAYS handoff to the appropriate agent.",
                    "Triage_Router", "Routes messages to specialists");

#pragma warning disable MAAIW001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                var handoffWorkflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triageRouter)
                    // Define the forward transition edges
                    .WithHandoffs(triageRouter, [networkAdmin, billingSupport])
                    // Define the reverse transition edges to return to triage if needed
                    .WithHandoffs([networkAdmin, billingSupport], triageRouter)
                    .Build();
#pragma warning restore MAAIW001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

                List<ChatMessage> conversation = [];
                while (true)
                {
                    Console.Write("\nEnterprise User: ");
                    conversation.Add(new ChatMessage(ChatRole.User, Console.ReadLine()!));

                    // The workflow manages the handoff and returns the updated state
                    var newMessages = await EnterpriseOrchestrator.RunWorkflowAsync(handoffWorkflow, conversation);
                    conversation.AddRange(newMessages);
                }

            case "4":
                // --- GROUP CHAT (COLLABORATIVE SWARM) ---
                // Agents converse in a shared context window until iteration limit is reached
                ChatClientAgent secOps = new(client, "You are SecOps. Focus on security liabilities.", "SecOps");
                ChatClientAgent devOps = new(client, "You are DevOps. Focus on uptime and deployment safety.", "DevOps");
                ChatClientAgent legalReview = new(client, "You are Legal. Focus on compliance.", "Legal");

                var groupChatWorkflow = AgentWorkflowBuilder.CreateGroupChatBuilderWith(
                        agents => new RoundRobinGroupChatManager(agents) { MaximumIterationCount = 4 }
                    )
                    .AddParticipants([secOps, devOps, legalReview])
                    .Build();

                await EnterpriseOrchestrator.RunWorkflowAsync(
                    groupChatWorkflow,
                    [new ChatMessage(ChatRole.User, "We need to push an emergency hotfix to the payment gateway database. Review the implications.")]
                );
                break;
        }
    }

    // Helper method to rapidly generate localization agents
    private static ChatClientAgent GetTranslationAgent(string targetLanguage, IChatClient chatClient) =>
        new(chatClient,
            $"You are a localization expert. Translate the input into {targetLanguage}. Prepend your response with '[{targetLanguage}]:'.",
            name: $"{targetLanguage}_Translator");
}
