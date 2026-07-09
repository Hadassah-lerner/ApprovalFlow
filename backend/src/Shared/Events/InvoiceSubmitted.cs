using Shared.Enums;

namespace Shared.Events
{ 
public class InvoiceSubmitted
{
    public Guid InvoiceId { get; init; }

    public string TrackingId { get; init; } = string.Empty;

    public string Submitter { get; init; } = string.Empty;

    public string Department { get; init; } = string.Empty;

    public string Vendor { get; init; } = string.Empty;

    public bool VendorKnown { get; init; }

    public string InvoiceNumber { get; init; } = string.Empty;

    public InvoiceCategory Category { get; init; }

    public Currency Currency { get; init; }

    public decimal Total { get; init; }

    public decimal TaxAmount { get; init; }

    public bool ReceiptPresent { get; init; }

    public DateTime InvoiceDate { get; init; }

    public string Notes { get; init; } = string.Empty;

    public DateTime SubmittedAt { get; init; } = DateTime.UtcNow;
}
}