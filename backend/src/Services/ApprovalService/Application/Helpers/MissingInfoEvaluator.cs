namespace ApprovalService.Application.Helpers
{
    using ApprovalService.Application.Features.ProcessInvoice;
    using System.Collections.Generic;

    public class MissingInfoResult
    {
        public bool HasMissingInfo => MissingFields.Count > 0;
        public List<string> MissingFields { get; set; } = new List<string>();
    }

    public class MissingInfoEvaluator
    {
        public MissingInfoResult Evaluate(WorkflowContext context)
        {
            var result = new MissingInfoResult();

            if (context.Invoice == null)
            {
                result.MissingFields.Add("Complete Invoice Data Structure");
                return result;
            }

            dynamic invoice = context.Invoice;

            try
            {
                if (string.IsNullOrWhiteSpace(invoice.VendorName))
                    result.MissingFields.Add("VendorName");

                if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
                    result.MissingFields.Add("InvoiceNumber");

                if (invoice.TotalAmount == null || invoice.TotalAmount <= 0)
                    result.MissingFields.Add("TotalAmount");

                if (invoice.LineItems == null || invoice.LineItems.Count == 0)
                    result.MissingFields.Add("LineItems");
            }
            catch
            {
                if (string.IsNullOrWhiteSpace(context.ProcessedInvoiceText))
                    result.MissingFields.Add("ProcessedInvoiceText (No textual data found)");
            }

            return result;
        }
    }
}
