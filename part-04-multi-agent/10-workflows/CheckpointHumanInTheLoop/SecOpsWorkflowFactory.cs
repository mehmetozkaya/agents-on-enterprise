using Microsoft.Agents.AI.Workflows;

namespace CheckpointHumanInTheLoop;

// 1. Define the strongly-typed communication payloads
public record SecurityIncidentState(string ThreatData, string ProposedAction = "", bool IsApproved = false, string Status = "Analyzing");
public record ApprovalRequest(string ProposedAction);
public record ApprovalResponse(bool IsApproved, string HumanComments);

// 2. The Workflow Factory
public static class SecOpsWorkflowFactory
{
    public static RequestPort<ApprovalRequest, ApprovalResponse> HitlPort { get; } = 
        RequestPort.Create<ApprovalRequest, ApprovalResponse>("CisoApprovalPort");

    public static Workflow BuildWorkflow()
    {        
        // Node 1: The AI Analysis Node - analyzes and prepares the approval request
        Func<SecurityIncidentState, ApprovalRequest> analyzerFunc = state =>
        {
            Console.WriteLine("[AI SecOps Agent] Analyzing threat logs...");
            // Simulated LLM logic: The agent decides the server must be isolated
            string proposedAction = "Quarantine Database Server DB-PROD-01";
            Console.WriteLine($"[AI SecOps Agent] Proposed action: {proposedAction}");
            return new ApprovalRequest(proposedAction);
        };
        var analyzerNode = analyzerFunc.BindAsExecutor("AnalyzerNode");

        // Bind the RequestPort as an executor for HITL
        var hitlNode = HitlPort.BindAsExecutor();

        // Node 2: The Action Execution Node - processes the approval response
        Func<ApprovalResponse, SecurityIncidentState> executionFunc = response =>
        {
            if (response.IsApproved)
            {
                Console.WriteLine($"[Execution Engine] EXECUTING ACTION: Quarantine Database Server DB-PROD-01");
                return new SecurityIncidentState(
                    ThreatData: "Multiple failed SSH logins detected on DB-PROD-01",
                    ProposedAction: "Quarantine Database Server DB-PROD-01",
                    IsApproved: true,
                    Status: "Resolved - Action Taken");
            }
            else
            {
                Console.WriteLine("[Execution Engine] ABORTING: Human manager denied the action.");
                return new SecurityIncidentState(
                    ThreatData: "Multiple failed SSH logins detected on DB-PROD-01",
                    ProposedAction: "Quarantine Database Server DB-PROD-01",
                    IsApproved: false,
                    Status: "Resolved - Aborted by Human");
            }
        };
        var executionNode = executionFunc.BindAsExecutor("ExecutionNode");

        // Build the Graph: Analyzer -> HITL Port -> Execution
        var builder = new WorkflowBuilder(analyzerNode)
            .AddEdge(analyzerNode, hitlNode)
            .AddEdge(hitlNode, executionNode);

        return builder.Build();
    }
}
