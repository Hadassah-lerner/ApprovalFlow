using Shared.Enums;

namespace SubmissionService.Api.Responses;

public class InvoiceDetailsResponse
{
    public Guid Id { get; set; }

    public string Submitter { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Vendor { get; set; } = string.Empty;

    public string InvoiceNumber { get; set; } = string.Empty;

    public InvoiceCategory Category { get; set; }

    public Currency Currency { get; set; }

    public decimal Total { get; set; }

    public decimal TaxAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? UpdatedAt { get; set; }
}