namespace SubmissionService.Application.Features.InvoiceSubmission.DTOs
{
    public class InvoiceResponse
    {
        public Guid Id { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
