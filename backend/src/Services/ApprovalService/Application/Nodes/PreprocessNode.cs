using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ProcessInvoice;
using ApprovalService.Api.Events;
using System.Text;

namespace ApprovalService.Application.Nodes
{
    public class PreprocessNode : IWorkflowNode
    {
        public async Task ExecuteAsync(WorkflowContext context)
        {
            var invoice = (InvoiceSubmittedEvent)context.Invoice;
            var sb = new StringBuilder();

            sb.AppendLine($"Invoice from: {invoice.Vendor}");
            sb.AppendLine($"Invoice Number: {invoice.InvoiceNumber}");
            sb.AppendLine($"Department: {invoice.Department}");
            sb.AppendLine($"Total Amount: {invoice.Total} {invoice.Currency}");
            sb.AppendLine("Line Items:");

            foreach (var item in invoice.LineItems)
            {
                decimal itemTotal = item.Quantity * item.UnitPrice;
                sb.AppendLine($"- {item.Description}: {item.Quantity} x {item.UnitPrice} = {itemTotal}");
            }

            context.ProcessedInvoiceText = sb.ToString();
        }
    }
}