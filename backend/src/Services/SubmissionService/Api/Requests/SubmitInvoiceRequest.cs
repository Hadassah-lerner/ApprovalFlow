using Shared.Enums;

namespace SubmissionService.Api.Requests;

public class SubmitInvoiceRequest
{
    public string Submitter { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Vendor { get; set; } = string.Empty;

    public bool VendorKnown { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public InvoiceCategory Category { get; set; }

    public Currency Currency { get; set; }

    public List<SubmitLineItemRequest> LineItems { get; set; } = [];

    public decimal TaxAmount { get; set; }

    public decimal Total { get; set; }

    public bool ReceiptPresent { get; set; }

    public DateTime InvoiceDate { get; set; }

    public string Notes { get; set; } = string.Empty;
}

public class SubmitLineItemRequest
{
    public string Description { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}