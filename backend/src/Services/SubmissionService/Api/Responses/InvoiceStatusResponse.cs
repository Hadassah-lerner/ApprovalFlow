namespace SubmissionService.Api.Responses;

public class InvoiceStatusResponse
{
    public Guid InvoiceId { get; set; }

    public string Status { get; set; } = string.Empty;
}