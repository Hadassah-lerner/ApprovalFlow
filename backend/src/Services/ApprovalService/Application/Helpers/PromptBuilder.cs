using System.Text;

namespace ApprovalService.Application.Helpers
{
    public class PromptBuilder
    {
        public string BuildClassificationPrompt(string processedInvoiceText, string policyText)
        {
            var sb = new StringBuilder();

            sb.AppendLine("### SYSTEM INSTRUCTIONS ###");
            sb.AppendLine("You are an expert AI Invoice Classifier Agent working inside an automated corporate ERP system.");
            sb.AppendLine("Your job is to analyze the provided invoice text against the corporate compliance POLICY provided below.");
            sb.AppendLine("You must strictly categorize the invoice and output your response in a valid JSON format ONLY.");
            sb.AppendLine();

            sb.AppendLine("### CORPORATE POLICY ###");
            sb.AppendLine(policyText);
            sb.AppendLine();

            sb.AppendLine("### INVOICE DATA TO ANALYZE ###");
            sb.AppendLine(processedInvoiceText);
            sb.AppendLine();

            sb.AppendLine("### EXPECTED OUTPUT FORMAT ###");
            sb.AppendLine("Return a single JSON object matching the schema below. Do not include any markdown comments, conversational filler or surrounding text outside the JSON block.");
            sb.AppendLine("{");
            sb.AppendLine("  \"category\": \"String (e.g., Infrastructure, Hardware, Marketing, Operations)\",");
            sb.AppendLine("  \"confidence\": 0.00, // Float between 0.0 and 1.0");
            sb.AppendLine("  \"summary\": \"Brief 1-sentence summary of what this invoice is for\",");
            sb.AppendLine("  \"detectedViolations\": [\"List of specific rules from the policy that this invoice potentially violates. Leave empty if none.\"]");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
