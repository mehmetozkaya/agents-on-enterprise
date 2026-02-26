using System;
using System.Threading.Tasks;
using Microsoft.Agents.AI.Workflows;

// 1. The Event Payload
public record CustomerPayload(string CompanyName, string Industry, bool IsValidated = false, string Status = "New");

class Program
{
    static async Task Main(string[] args)
    {
        // 2a. The Validation Node
        Func<CustomerPayload, CustomerPayload> validateFunc = payload =>
        {
            Console.WriteLine($"[Validator] Inspecting payload for: {payload.CompanyName}");
            bool isValid = !string.IsNullOrWhiteSpace(payload.CompanyName);
            return payload with { IsValidated = isValid, Status = isValid ? "Validated" : "Rejected" };
        };
        var validatorExecutor = validateFunc.BindAsExecutor("ValidationNode");

        // 2b. The Enrichment Node
        Func<CustomerPayload, CustomerPayload> enrichFunc = payload =>
        {
            Console.WriteLine($"[Enricher] Applying '{payload.Industry}' enterprise templates...");
            return payload with { Status = "Enriched" };
        };
        var enricherExecutor = enrichFunc.BindAsExecutor("EnrichmentNode");

        // 2c. The Audit Node
        Func<CustomerPayload, CustomerPayload> auditFunc = payload =>
        {
            Console.WriteLine($"[Auditor] Logging final state to database. Final Status: {payload.Status}");
            return payload;
        };
        var auditExecutor = auditFunc.BindAsExecutor("AuditNode");

        // 3. Construct the Workflow Graph
        var workflow = new WorkflowBuilder(validatorExecutor)
            // Conditional Edge: Only enrich if valid
            .AddEdge<CustomerPayload>(validatorExecutor, enricherExecutor, condition: p => p?.IsValidated == true)
            // Conditional Edge: If invalid, skip to audit
            .AddEdge<CustomerPayload>(validatorExecutor, auditExecutor, condition: p => p?.IsValidated == false)
            // Standard Edge: Enrichment always flows to Audit
            .AddEdge(enricherExecutor, auditExecutor)
            .Build();

        Console.WriteLine("--- Starting Workflow Execution ---\n");

        var initialPayload = new CustomerPayload("Contoso Pharmaceuticals", "Healthcare");

        // 4. Execute the Graph
        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, initialPayload);

        // Listen to the stream to observe the nodes completing their work
        await foreach (WorkflowEvent evt in run.WatchStreamAsync())
        {
            if (evt is ExecutorCompletedEvent executorComplete)
            {
                Console.WriteLine($"[System] -> Node '{executorComplete.ExecutorId}' completed successfully.\n");
            }
        }

        Console.WriteLine("--- Workflow Complete ---");
    }
}