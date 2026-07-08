using Shared.Enums;

namespace ApprovalService.Api.Events
{
    public record InvoiceSubmittedEvent(
    Guid Id,
    string TrackingId,
    string Submitter,
    string Department,
    string Vendor,
    bool VendorKnown,
    string InvoiceNumber,
    InvoiceCategory Category,
    Currency Currency,
    List<LineItemEvent> LineItems,
    decimal TaxAmount,
    decimal Total,
    bool ReceiptPresent,
    DateTime InvoiceDate,
    string Notes
);

        public record LineItemEvent(
            string Description,
            int Quantity,
            decimal UnitPrice
        );
    }

