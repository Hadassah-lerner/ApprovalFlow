namespace ApprovalService.Application.Features.ProcessInvoice
{
    using System.Collections.Generic;

    public class WorkflowContext
    {
        public object Invoice { get; set; } 
        public string ProcessedInvoiceText { get; set; } 
        public string FinalPrompt { get; set; } 
        public string AiRawResult { get; set; }
        public string Category { get; set; }
        public double Confidence { get; set; }
        public string FinalDecision { get; set; } 
        public List<string> PolicyViolations { get; set; } = new List<string>();
        public bool RequiresHumanReview => FinalDecision == "HumanReview";
        public string PolicyText { get; set; } = string.Empty;
    }
}
