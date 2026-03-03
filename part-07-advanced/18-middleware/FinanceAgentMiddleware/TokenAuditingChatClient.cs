using Microsoft.Extensions.AI;

namespace FinanceAgentMiddleware.Middleware;

/// <summary>
/// Token Auditing Middleware - Logs all token consumption for cost tracking.
/// </summary>
public class TokenAuditingMiddleware : DelegatingChatClient
{
    public TokenAuditingMiddleware(IChatClient innerClient) : base(innerClient) { }

    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("[TokenAudit] Request intercepted...");

        var response = await base.GetResponseAsync(chatMessages, options, cancellationToken);

        if (response.Usage != null)
        {
            Console.WriteLine($"[TokenAudit] Tokens -> Input: {response.Usage.InputTokenCount}, Output: {response.Usage.OutputTokenCount}");
        }

        return response;
    }
}
