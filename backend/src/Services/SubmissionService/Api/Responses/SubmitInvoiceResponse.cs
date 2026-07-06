namespace SubmissionService.Api.Responses;

public class SubmitInvoiceResponse
{
    public Guid InvoiceId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}