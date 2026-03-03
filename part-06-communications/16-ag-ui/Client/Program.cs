using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;


Console.WriteLine($"Connecting to AG-UI server\n");

// 1. Connect to the remote AG-UI Server
using var httpClient = new HttpClient();

var aguiClient = new AGUIChatClient(
    httpClient,
    "http://localhost:5000/agui/support",
    NullLoggerFactory.Instance);

// 2. Wrap the remote connection in a local Agent interface
AIAgent clientAgent = aguiClient.AsAIAgent(
    name: "SupportClient",
    description: "You are a helpful enterprise support agent.");

Console.WriteLine("User: How can I reset my corporate password?");
Console.Write("Agent: ");

// 3. Send the message and stream the UI-compliant response in real-time
await foreach (var update in clientAgent.RunStreamingAsync("How can I reset my corporate password?"))
{
    Console.Write(update.Text);
}