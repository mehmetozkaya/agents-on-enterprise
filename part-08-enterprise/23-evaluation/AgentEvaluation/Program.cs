using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Text.Json;

Console.WriteLine("--- Starting Enterprise Agent Evaluation Suite ---\n");

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

var credential = new AzureCliCredential();

// 1. Initialize the Target Agent (The agent being tested - e.g., gpt-4o-mini)
IChatClient targetClient = new AzureOpenAIClient(new Uri(endpoint), credential)
    .GetChatClient(deploymentName).AsIChatClient();

AIAgent targetAgent = targetClient.AsAIAgent(
    name: "SupportAgent",
    instructions: "You are a helpful IT support agent. Be concise."
);

// 2. Initialize the Judge Agent (A superior reasoning model - e.g., gpt-4o or o1)-for now we have only gpt-5-mini
IChatClient judgeClient = new AzureOpenAIClient(new Uri(endpoint), credential)
    .GetChatClient(deploymentName).AsIChatClient();

// We instruct the Judge to act as an objective evaluator and output strict JSON.
AIAgent judgeAgent = judgeClient.AsAIAgent(
    name: "EvaluationJudge",
    instructions: @"You are an expert AI evaluator. 
            Analyze the Interaction and grade the Target Agent's response from 1 to 5.
            Output ONLY valid JSON matching this schema: 
            { 'RelevanceScore': int, 'GroundednessScore': int, 'Reasoning': 'string' }"
);

// 3. Define a scenario from our 'Golden Set'
string userPrompt = "How do I reset my VPN password?";
string injectedRAGContext = "To reset the VPN password, navigate to portal.contoso.com and click 'Security'.";

// Execute the Target Agent (Capturing its response)
string fullPrompt = $"Context: {injectedRAGContext}\nUser Query: {userPrompt}";
AgentResponse targetResponse = await targetAgent.RunAsync(fullPrompt);

Console.WriteLine($"[Target Agent Output]: {targetResponse.Text}\n");

// 4. Formulate the Evaluation Prompt for the Judge
string evaluationTask = $@"
            Evaluate this interaction based on the provided context.
            
            [RAG Context Provided]: {injectedRAGContext}
            [User Prompt]: {userPrompt}
            [Agent Output]: {targetResponse.Text}
            
            Provide scores for Relevance (Does it answer the prompt directly?) 
            and Groundedness (Is it strictly based on the RAG context?).";

// 5. Execute the Judge Agent
AgentResponse judgeResponse = await judgeAgent.RunAsync(evaluationTask);

Console.WriteLine($"[Judge Evaluation Raw JSON]:\n{judgeResponse.Text}\n");

// 6. Assertions (The CI/CD Gatekeeper)
try
{
    // Parse the JSON output from the Judge
    using JsonDocument doc = JsonDocument.Parse(judgeResponse.Text);
    int relevance = doc.RootElement.GetProperty("RelevanceScore").GetInt32();
    int groundedness = doc.RootElement.GetProperty("GroundednessScore").GetInt32();

    Console.WriteLine($"Parsed Scores -> Relevance: {relevance}/5 | Groundedness: {groundedness}/5");

    // Define our enterprise acceptable thresholds
    if (relevance < 4 || groundedness < 4)
    {
        throw new Exception($"EVALUATION FAILED: Agent cognitive performance dropped below enterprise thresholds. Reason: {doc.RootElement.GetProperty("Reasoning").GetString()}");
    }

    Console.WriteLine("--- EVALUATION PASSED: Build Approved for Deployment ---");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
    Console.ResetColor();
    // In a real pipeline, Environment.Exit(1) would fail the build here.
}