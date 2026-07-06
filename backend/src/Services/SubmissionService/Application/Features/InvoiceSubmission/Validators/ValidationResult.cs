namespace SubmissionService.Application.Features.InvoiceSubmission.Validators
{
    public class ValidationResult
    {
        public bool IsValid { get; set; } = true;
        public List<string> Errors { get; set; } = new();
    }
}
