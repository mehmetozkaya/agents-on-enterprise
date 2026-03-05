using Microsoft.Extensions.AI;

namespace SecurityArchitecture;

/// <summary>
/// Enterprise Security Middleware - Implements input/output guardrails for AI agents.
/// Uses the DelegatingChatClient pattern for middleware pipeline composition.
/// </summary>
public class EnterpriseSecurityMiddleware : DelegatingChatClient
{
    // Patterns that indicate prompt injection attempts
    private static readonly string[] InjectionPatterns =
    [
        "ignore all previous instructions",
        "ignore all instructions",
        "forget everything above",
        "reveal system prompt",
        "bypass security",
        "disregard your rules"
    ];

    // Patterns that indicate sensitive enterprise data (DLP)
    private static readonly string[] SensitiveDataPatterns =
    [
        "SSN:",
        "SSN-",
        "social security",
        "credit card",
        "bank account"
    ];

    public EnterpriseSecurityMiddleware(IChatClient innerClient) : base(innerClient) { }

    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("[Security] Intercepting inbound request for evaluation...");

        // 1. INPUT GUARDRAIL: Detect prompt injection attempts
        foreach (var message in chatMessages.Where(m => m.Role == ChatRole.User))
        {
            var text = GetMessageText(message).ToLowerInvariant();

            if (await AnalyzeForPromptInjectionAsync(text))
            {
                Console.WriteLine("[Security] CRITICAL: Prompt Injection detected. Blocking execution.");
                return new ChatResponse([new ChatMessage(ChatRole.Assistant,
                    "Security Violation: This request has been blocked by enterprise safety policies.")]);
            }
        }

        Console.WriteLine("[Security] Input validation passed.");

        // 2. Execute the Agent's cognitive loop
        var response = await base.GetResponseAsync(chatMessages, options, cancellationToken);

        // 3. OUTPUT GUARDRAIL: Data Leakage Prevention (DLP)
        Console.WriteLine("[Security] Intercepting outbound response for DLP scanning...");
        var responseText = GetResponseText(response);

        if (await ContainsSensitiveEnterpriseDataAsync(responseText))
        {
            Console.WriteLine("[Security] WARNING: Agent attempted to leak sensitive data. Redacting.");
            return new ChatResponse([new ChatMessage(ChatRole.Assistant,
                "[REDACTED BY PURVIEW: Response contained sensitive corporate data.]")]);
        }

        Console.WriteLine("[Security] Output validation passed.");
        return response;
    }

    /// <summary>
    /// Analyzes text for prompt injection patterns.
    /// In production, integrate with Azure AI Content Safety Prompt Shield.
    /// </summary>
    private static Task<bool> AnalyzeForPromptInjectionAsync(string text)
    {
        var isInjection = InjectionPatterns.Any(pattern => text.Contains(pattern));
        return Task.FromResult(isInjection);
    }

    /// <summary>
    /// Scans text for sensitive enterprise data patterns.
    /// In production, integrate with Microsoft Purview DLP policies.
    /// </summary>
    private static Task<bool> ContainsSensitiveEnterpriseDataAsync(string text)
    {
        var containsSensitive = SensitiveDataPatterns.Any(pattern =>
            text.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(containsSensitive);
    }

    private static string GetMessageText(ChatMessage msg) =>
        string.Join(" ", msg.Contents.OfType<TextContent>().Select(c => c.Text ?? ""));

    private static string GetResponseText(ChatResponse response) =>
        string.Join(" ", response.Messages
            .SelectMany(m => m.Contents.OfType<TextContent>())
            .Select(c => c.Text ?? ""));
}
