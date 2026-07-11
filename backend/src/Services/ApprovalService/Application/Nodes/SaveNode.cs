using ApprovalService.Api.Events;
using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ProcessInvoice;
using ApprovalService.Domain.Entities;
using ApprovalService.Infrastructure.Persistence;

namespace ApprovalService.Application.Nodes
{
    public class SaveNode : IWorkflowNode
    {
        private readonly ApprovalDbContext _dbContext;

        public SaveNode(ApprovalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteAsync(WorkflowContext context)
        {
            var invoiceEvent = (InvoiceSubmittedEvent)context.Invoice;

            var dbLineItems = invoiceEvent.LineItems.Select(item =>
                new LineItemApproval(
                    item.Description,
                    item.Quantity,
                    item.UnitPrice
                )
            ).ToList();

            var approvalRecord = new InvoiceApproval(
                            invoiceEvent.Id,
                            invoiceEvent.TrackingId,
                            invoiceEvent.Submitter,
                            invoiceEvent.Department,
                            invoiceEvent.Vendor,
                            invoiceEvent.VendorKnown,
                            invoiceEvent.InvoiceNumber,
                            invoiceEvent.Category,
                            invoiceEvent.Currency,
                            dbLineItems,
                            invoiceEvent.TaxAmount,
                            invoiceEvent.Total,
                            invoiceEvent.ReceiptPresent,
                            invoiceEvent.InvoiceDate,
                            invoiceEvent.Notes
                        );

            string violationsSummary = string.Join(" | ", context.PolicyViolations);

            approvalRecord.SetAiTriageResults(
                urgency: context.Confidence < 0.5 ? "High" : "Medium", // לוגיקת דחיפות בסיסית
                suggestedCategory: context.Category ?? "Other",
                reasoning: $"Decision: {context.FinalDecision}. Violations: {violationsSummary}"
            );
            if (context.FinalDecision == "Approved")
            {
                approvalRecord.ChangeStatus(Shared.Enums.ApprovalStatus.Approved);
            }
            else
            {
                approvalRecord.ChangeStatus(Shared.Enums.ApprovalStatus.Pending);
            }
            await _dbContext.InvoiceApprovals.AddAsync(approvalRecord);
            await _dbContext.SaveChangesAsync();
        }
    }
}
