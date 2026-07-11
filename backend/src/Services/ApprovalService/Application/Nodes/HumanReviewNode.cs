using ApprovalService.Api.Controllers;
using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ProcessInvoice;

namespace ApprovalService.Application.Nodes
{
    public class HumanReviewNode : IWorkflowNode
    {
        private readonly ILogger<HumanReviewNode> _logger;

        public HumanReviewNode(ILogger<HumanReviewNode> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(WorkflowContext context)
        {
            if (context.FinalDecision == "HumanReview")
                _logger.LogInformation($"[HumanReview] Invoice flagged for manual check. Reason: {string.Join(", ", context.PolicyViolations)}");
        }
    }
}

