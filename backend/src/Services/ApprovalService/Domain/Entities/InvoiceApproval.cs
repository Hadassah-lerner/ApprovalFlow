using Shared.Enums;

namespace ApprovalService.Domain.Entities
{
    public class InvoiceApproval
    {
        public Guid Id { get; private set; }
        public string TrackingId { get; private set; } = string.Empty;
        public string Submitter { get; private set; } = string.Empty;
        public string Department { get; private set; } = string.Empty;
        public string Vendor { get; private set; } = string.Empty;
        public bool VendorKnown { get; private set; }
        public string InvoiceNumber { get; private set; } = string.Empty;
        public InvoiceCategory Category { get; private set; }
        public Currency Currency { get; private set; }
        public List<LineItemApproval> LineItems { get; private set; } = [];
        public decimal TaxAmount { get; private set; }
        public decimal Total { get; private set; }
        public bool ReceiptPresent { get; private set; }
        public DateTime InvoiceDate { get; private set; }
        public string Notes { get; private set; }  = string.Empty;
        public ApprovalStatus ApprovalStatus { get; private set; }
        public string? AiUrgencyLevel { get; private set; } 
        public string? AiSuggestedCategory { get; private set; } 
        public string? AiReasoning { get; private set; }


        public decimal LineItemsTotal =>
        LineItems.Sum(x => x.Total);

        public InvoiceApproval(
            Guid originalInvoiceId,
            string? trackingId,
            string submitter,
            string department,
            string vendor,
            bool vendorKnown,
            string invoiceNumber,
            InvoiceCategory category,
            Currency currency,
            List<LineItemApproval> lineItems,
            decimal taxAmount,
            decimal total,
            bool receiptPresent,
            DateTime invoiceDate,
            string notes)
        {
            Id = originalInvoiceId;
            TrackingId = !string.IsNullOrWhiteSpace(trackingId)
                        ? trackingId
                        : $"INV-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            Submitter = submitter;
            Department = department;
            Vendor = vendor;
            VendorKnown = vendorKnown;
            InvoiceNumber = invoiceNumber;
            Category = category;
            Currency = currency;
            LineItems = lineItems;
            TaxAmount = taxAmount;
            Total = total;
            ReceiptPresent = receiptPresent;
            InvoiceDate = invoiceDate;
            Notes = notes;

            ApprovalStatus = ApprovalStatus.Pending;
        }

        public void ChangeStatus(ApprovalStatus status)
        {
            ApprovalStatus = status;
        }

        public void SetAiTriageResults(string urgency, string suggestedCategory, string reasoning)
        {
            AiUrgencyLevel = urgency;
            AiSuggestedCategory = suggestedCategory;
            AiReasoning = reasoning;
        }

        public InvoiceApproval()
        {
        }

    }
}
