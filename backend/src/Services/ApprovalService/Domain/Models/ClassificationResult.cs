namespace ApprovalService.Domain.Models
{
    public class ClassificationResult
    {
        public string SuggestedCategory { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
        public string Reasoning { get; set; } = string.Empty;
        public List<string> DetectedViolations { get; set; } = new List<string>();
    }
}