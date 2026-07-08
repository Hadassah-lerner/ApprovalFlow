namespace ApprovalService.Domain.Models
{
    public class ClassificationResult
    {
        public string UrgencyLevel { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Reasoning { get; set; } = string.Empty;
    }
}
