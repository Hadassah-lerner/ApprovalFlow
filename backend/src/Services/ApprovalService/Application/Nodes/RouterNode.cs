using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ProcessInvoice;
using ApprovalService.Api.Events;
using System;
using System.Threading.Tasks;

namespace ApprovalService.Application.Nodes
{
    public class RouterNode : IWorkflowNode
    {
        private readonly IPolicyLoader _policyLoader;

        public RouterNode(IPolicyLoader policyLoader)
        {
            _policyLoader = policyLoader;
        }

        public async Task ExecuteAsync(WorkflowContext context)
        {
            var invoice = context.Invoice as InvoiceSubmittedEvent;
            if (invoice == null)
            {
                context.FinalDecision = "HumanReview";
                context.PolicyViolations.Add("SYSTEM-ERROR: Invalid or missing invoice data in context.");
                return;
            }

            decimal amountInUsd = invoice.Total;
            decimal autonomyCeiling = _policyLoader.GetDecimalThreshold("AUTONOMY-CEILING", 250.00m);
            decimal autonomyConfidence = _policyLoader.GetDecimalThreshold("AUTONOMY-CONFIDENCE", 0.80m);
            decimal saasLimit = _policyLoader.GetSaaSLimit(200.00m);

            if (amountInUsd > autonomyCeiling)
            {
                context.PolicyViolations.Add($"AUTONOMY-CEILING: Amount exceeds the current policy ceiling of ${autonomyCeiling}.");
            }

            if (!string.IsNullOrEmpty(context.SuggestedCategory) &&
                context.SuggestedCategory.Equals("Software", StringComparison.OrdinalIgnoreCase) &&
                amountInUsd > saasLimit)
            {
                context.PolicyViolations.Add($"SAAS-01: Software subscription exceeds the allowed ${saasLimit} / month.");
            }

            if ((decimal)context.Confidence < autonomyConfidence)
            {
                context.PolicyViolations.Add($"AUTONOMY-CONFIDENCE: AI confidence score ({context.Confidence}) is below required {autonomyConfidence}.");
            }

            if (!invoice.VendorKnown)
            {
                context.PolicyViolations.Add("GLOBAL-VENDOR: New or unknown vendor always forces manual review.");
            }

            if (context.PolicyViolations.Count > 0)
            {
                context.FinalDecision = "HumanReview";
            }
            else
            {
                context.FinalDecision = "Approved";
            }
        }
    }
}