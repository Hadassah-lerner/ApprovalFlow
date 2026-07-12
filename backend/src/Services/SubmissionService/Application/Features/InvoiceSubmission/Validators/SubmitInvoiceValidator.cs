using SubmissionService.Application.Features.InvoiceSubmission.DTOs;

namespace SubmissionService.Application.Features.InvoiceSubmission.Validators
{
    public static class SubmitInvoiceValidator
    {
        public static ValidationResult Validate(CreateInvoiceRequest request, DateTime utcNow)
        {
            var result = new ValidationResult();

            if (request == null)
            {
                result.Errors.Add("Request cannot be null");
                result.IsValid = false;
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.Submitter))
                result.Errors.Add("Submitter is required");

            if (string.IsNullOrWhiteSpace(request.Department))
                result.Errors.Add("Department is required");

            if (string.IsNullOrWhiteSpace(request.Vendor))
                result.Errors.Add("Vendor is required");

            if (string.IsNullOrWhiteSpace(request.InvoiceNumber))
                result.Errors.Add("Invoice number is required");

            if (request.LineItems == null || !request.LineItems.Any())
            {
                result.Errors.Add("Invoice must contain at least one line item");
            }
            else
            {
                var calculatedTotal =
                    request.LineItems.Sum(x => x.Quantity * x.UnitPrice) + request.TaxAmount;

                if (Math.Abs(calculatedTotal - request.Total) > 0.01m)
                    result.Errors.Add("Invoice total does not match line items.");
            }

            if (request.Total <= 0)
                result.Errors.Add("Total must be greater than zero");

            if (request.InvoiceDate == default)
                result.Errors.Add("Invoice date is required");

            if (request.InvoiceDate > utcNow)
                result.Errors.Add("Invoice date cannot be in the future.");

            result.IsValid = !result.Errors.Any();
            return result;
        }
    }
}