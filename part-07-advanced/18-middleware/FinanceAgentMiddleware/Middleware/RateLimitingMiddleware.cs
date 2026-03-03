using System.Collections.Concurrent;
using Microsoft.Extensions.AI;

namespace FinanceAgentMiddleware.Middleware;

/// <summary>
/// Rate Limiting Middleware - Implements token bucket algorithm for throttling.
/// </summary>
public class RateLimitingMiddleware : DelegatingChatClient
{
    private readonly ConcurrentDictionary<string, (int Count, DateTime ResetTime)> _requests = new();
    private readonly int _maxRequestsPerMinute;

    public RateLimitingMiddleware(IChatClient innerClient, int maxRequestsPerMinute = 10) 
        : base(innerClient)
    {
        _maxRequestsPerMinute = maxRequestsPerMinute;
    }

    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var clientId = "default";
        var now = DateTime.UtcNow;

        var (count, resetTime) = _requests.GetOrAdd(clientId, _ => (0, now.AddMinutes(1)));

        // Reset counter if window expired
        if (now >= resetTime)
        {
            _requests[clientId] = (1, now.AddMinutes(1));
            Console.WriteLine($"[RateLimit] Request allowed ({1}/{_maxRequestsPerMinute})");
        }
        else if (count >= _maxRequestsPerMinute)
        {
            Console.WriteLine($"[RateLimit] BLOCKED - Limit exceeded ({count}/{_maxRequestsPerMinute})");
            return new ChatResponse([new ChatMessage(ChatRole.Assistant, 
                "Rate limit exceeded. Please try again later.")]);
        }
        else
        {
            _requests[clientId] = (count + 1, resetTime);
            Console.WriteLine($"[RateLimit] Request allowed ({count + 1}/{_maxRequestsPerMinute})");
        }

        return await base.GetResponseAsync(chatMessages, options, cancellationToken);
    }
}
