using CheckpointHumanInTheLoop;
using Microsoft.Agents.AI.Workflows;

class Program
{
    static async Task Main(string[] args)
    {
        // Instantiate the workflow from our custom factory
        var workflow = SecOpsWorkflowFactory.BuildWorkflow();

        // Initialize the Checkpoint Manager to capture state snapshots
        var checkpointManager = CheckpointManager.Default;
        var checkpoints = new List<CheckpointInfo>();

        Console.WriteLine("--- SecOps Pipeline Initiated ---");
        var initialState = new SecurityIncidentState("Multiple failed SSH logins detected on DB-PROD-01");

        // Start the workflow
        await using StreamingRun checkpointedRun = await InProcessExecution
            .RunStreamingAsync(workflow, initialState, checkpointManager);

        await foreach (WorkflowEvent evt in checkpointedRun.WatchStreamAsync())
        {
            switch (evt)
            {
                // THE PAUSE: The workflow has hit the RequestPort and needs human intervention
                case RequestInfoEvent requestInputEvt:
                    ApprovalResponse response = HandleHumanApproval(requestInputEvt.Request);
                    // Inject the human's decision back into the frozen workflow
                    await checkpointedRun.SendResponseAsync(requestInputEvt.Request.CreateResponse(response));
                    break;

                // THE CHECKPOINT: The framework has captured the state snapshot after a superstep
                case SuperStepCompletedEvent superStepCompletedEvt:
                    CheckpointInfo? checkpoint = superStepCompletedEvt.CompletionInfo?.Checkpoint;
                    if (checkpoint is not null)
                    {
                        checkpoints.Add(checkpoint);
                        Console.WriteLine($"[Database] -> Checkpoint securely saved at step {checkpoints.Count}.");
                    }
                    break;

                case WorkflowOutputEvent workflowOutputEvt:
                    var finalState = workflowOutputEvt.As<SecurityIncidentState>();
                    Console.WriteLine($"\n[Workflow Terminated] Final Status: {finalState?.Status}");
                    break;
            }
        }

        // --- DEMONSTRATING FAULT RECOVERY (TIME TRAVEL) ---
        // Imagine the CISO accidentally hit "Deny", realized the threat was real, and needs to undo the decision.
        // We can simply reload the checkpoint captured right before the human was asked!

        Console.WriteLine("\n--- Rewinding system to before human input (Checkpoint 1) ---");
        CheckpointInfo preApprovalCheckpoint = checkpoints[0]; // The state right before RequestPort

        // Restore the state directly into the run instance
        await checkpointedRun.RestoreCheckpointAsync(preApprovalCheckpoint, CancellationToken.None);

        // Resume the stream from the restored point in time
        await foreach (WorkflowEvent evt in checkpointedRun.WatchStreamAsync())
        {
            switch (evt)
            {
                case RequestInfoEvent requestInputEvt:
                    Console.WriteLine("\n[System] Re-prompting human due to checkpoint restoration...");
                    ApprovalResponse response = HandleHumanApproval(requestInputEvt.Request);
                    await checkpointedRun.SendResponseAsync(requestInputEvt.Request.CreateResponse(response));
                    break;
                case WorkflowOutputEvent workflowOutputEvt:
                    var finalState = workflowOutputEvt.As<SecurityIncidentState>();
                    Console.WriteLine($"\n[Workflow Terminated] New Final Status: {finalState?.Status}");
                    break;
            }
        }
    }

    // Helper method to simulate a web dashboard or email approval process
    private static ApprovalResponse HandleHumanApproval(ExternalRequest request)
    {
        if (request.TryGetDataAs<ApprovalRequest>(out var approvalRequest))
        {
            Console.WriteLine($"\n*** HUMAN AUTHORIZATION REQUIRED ***");
            Console.WriteLine($"AI Proposes: {approvalRequest.ProposedAction}");
            Console.Write("Do you approve this destructive action? (Y/N): ");

            string? input = Console.ReadLine();
            bool isApproved = input?.Trim().ToUpper() == "Y";

            Console.Write("Provide justification comments: ");
            string comments = Console.ReadLine() ?? "No comments provided.";

            return new ApprovalResponse(isApproved, comments);
        }

        throw new NotSupportedException("Unknown request type.");
    }
}