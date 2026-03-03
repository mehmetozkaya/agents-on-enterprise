using Microsoft.Extensions.AI;

namespace FinanceAgentMiddleware.Middleware;

/// <summary>
/// Guardrail Middleware - Blocks prompt injection and malicious content.
/// </summary>
public class GuardrailMiddleware : DelegatingChatClient
{
    private static readonly string[] BlockedPatterns =
    [
        "ignore all previous instructions",
        "forget everything above",
        "reveal system prompt",
        "bypass security"
    ];

    public GuardrailMiddleware(IChatClient innerClient) : base(innerClient) { }

    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Validate input messages
        foreach (var message in chatMessages.Where(m => m.Role == ChatRole.User))
        {
            var text = GetText(message).ToLowerInvariant();

            foreach (var pattern in BlockedPatterns)
            {
                if (text.Contains(pattern))
                {
                    Console.WriteLine($"[Guardrail] BLOCKED: Detected '{pattern}'");
                    return new ChatResponse([new ChatMessage(ChatRole.Assistant, 
                        "I cannot process this request due to security policies.")]);
                }
            }
        }

        Console.WriteLine("[Guardrail] Input validation passed.");
        return await base.GetResponseAsync(chatMessages, options, cancellationToken);
    }

    private static string GetText(ChatMessage msg) =>
        string.Join(" ", msg.Contents.OfType<TextContent>().Select(c => c.Text ?? ""));
}
