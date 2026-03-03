using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;

namespace FinanceAgentMiddleware.Middleware;

/// <summary>
/// PII Redaction Middleware - Detects and redacts sensitive data (SSN, email, phone, etc.)
/// </summary>
public class PIIRedactionMiddleware : DelegatingChatClient
{
    public PIIRedactionMiddleware(IChatClient innerClient) : base(innerClient) { }

    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Redact PII from inbound messages
        var sanitizedMessages = chatMessages.Select(RedactMessage).ToList();
        Console.WriteLine("[PII] Inbound messages scanned.");

        var response = await base.GetResponseAsync(sanitizedMessages, options, cancellationToken);

        // Redact PII from outbound response
        foreach (var message in response.Messages)
            RedactMessage(message);

        Console.WriteLine("[PII] Outbound response scanned.");
        return response;
    }

    private static ChatMessage RedactMessage(ChatMessage message)
    {
        foreach (var content in message.Contents.OfType<TextContent>())
        {
            var text = content.Text ?? "";
            text = Regex.Replace(text, @"\b\d{3}-\d{2}-\d{4}\b", "[SSN]");           // SSN
            text = Regex.Replace(text, @"\b[\w.+-]+@[\w.-]+\.\w{2,}\b", "[EMAIL]");  // Email
            text = Regex.Replace(text, @"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b", "[PHONE]"); // Phone
            content.Text = text;
        }
        return message;
    }
}
