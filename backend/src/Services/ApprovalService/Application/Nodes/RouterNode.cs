using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ProcessInvoice;
using ApprovalService.Api.Events;

namespace ApprovalService.Application.Nodes
{
    public class RouterNode : IWorkflowNode
    {
        public async Task ExecuteAsync(WorkflowContext context)
        {
            var invoice = (InvoiceSubmittedEvent)context.Invoice;
            decimal amountInUsd = invoice.Total; 
            if (amountInUsd > 250)
            {
                context.FinalDecision = "HumanReview";
                context.PolicyViolations.Add("AUTONOMY-CEILING: Amount exceeds $250 ceiling for auto-approval.");
            }

            if (context.Confidence < 0.80)
            {
                context.FinalDecision = "HumanReview";
                context.PolicyViolations.Add("AUTONOMY-CONFIDENCE: AI confidence score is below 0.80.");
            }

            if (!invoice.VendorKnown)
            {
                context.FinalDecision = "HumanReview";
                context.PolicyViolations.Add("GLOBAL-VENDOR: New or unknown vendor always forces manual review.");
            }

            if (context.PolicyViolations.Count > 0)
            {
                context.FinalDecision = "HumanReview";
            }

            if (string.IsNullOrEmpty(context.FinalDecision))
            {
                context.FinalDecision = "Approved";
            }
        }
    }
}