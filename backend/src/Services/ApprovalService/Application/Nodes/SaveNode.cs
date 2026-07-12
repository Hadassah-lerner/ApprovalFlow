using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ProcessInvoice;
using ApprovalService.Domain.Interfaces;
using ApprovalService.Domain.Entities;
using ApprovalService.Api.Events;
using Shared.Enums;
using System.Threading.Tasks;
using ApprovalService.Application.Interfaces;

namespace ApprovalService.Application.Nodes
{
    public class SaveNode : IWorkflowNode
    {
        private readonly IInvoiceRepository _repository;

        public SaveNode(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(WorkflowContext context)
        {
            var invoice = context.Invoice as InvoiceSubmittedEvent;
            if (invoice == null) return;

            var existingApproval = await _repository.GetByIdAsync(invoice.Id);

            var targetStatus = context.FinalDecision == "HumanReview"
                ? ApprovalStatus.HumanReview
                : ApprovalStatus.Approved;

            if (existingApproval != null)
            {
                existingApproval.ChangeStatus(targetStatus);
            }
            else
            {
                var approvalLineItems = invoice.LineItems.Select(item =>
                    new LineItemApproval(item.Description, item.Quantity, item.UnitPrice)
                ).ToList();

                var newApproval = new InvoiceApproval(
                    invoice.Id,
                    invoice.TrackingId,
                    invoice.Submitter,
                    invoice.Department,
                    invoice.Vendor,
                    invoice.VendorKnown,
                    invoice.InvoiceNumber,
                    invoice.Category,
                    invoice.Currency,
                    approvalLineItems,
                    invoice.TaxAmount,
                    invoice.Total,
                    invoice.ReceiptPresent,
                    invoice.InvoiceDate,
                    invoice.Notes
                );

                newApproval.ChangeStatus(targetStatus);
                await _repository.AddAsync(newApproval);
            }

            await _repository.SaveChangesAsync();
        }
    }
}