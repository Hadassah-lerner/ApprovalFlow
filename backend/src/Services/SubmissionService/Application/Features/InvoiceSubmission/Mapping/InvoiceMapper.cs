using SubmissionService.Application.Features.InvoiceSubmission.DTOs;
using SubmissionService.Domain.Entities;

namespace SubmissionService.Application.Features.InvoiceSubmission.Mapping
{
    public static class InvoiceMapper
    {
        public static Invoice ToEntity(CreateInvoiceRequest request)
        {
            var lineItems = request.LineItems
                .Select(x => new LineItem(x.Description, x.Quantity, x.UnitPrice))
                .ToList();

            return new Invoice(
                request.Id, 
                request.Submitter,
                request.Department,
                request.Vendor,
                request.VendorKnown,
                request.InvoiceNumber,
                request.Category,
                request.Currency,
                lineItems,
                request.TaxAmount,
                request.Total,
                request.ReceiptPresent,
                request.InvoiceDate,
                request.Notes);
        }
    }
}