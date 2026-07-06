using SubmissionService.Api.Requests;
using SubmissionService.Api.Responses;
using SubmissionService.Application.Features.InvoiceSubmission.DTOs;

namespace SubmissionService.Api.Mapping;

public static class ApiMapper
{
    public static CreateInvoiceRequest ToApplication(
        SubmitInvoiceRequest request)
    {
        return new CreateInvoiceRequest
        {
            Submitter = request.Submitter,
            Department = request.Department,
            Vendor = request.Vendor,
            VendorKnown = request.VendorKnown,
            InvoiceNumber = request.InvoiceNumber,
            Category = request.Category,
            Currency = request.Currency,
            TaxAmount = request.TaxAmount,
            Total = request.Total,
            ReceiptPresent = request.ReceiptPresent,
            InvoiceDate = request.InvoiceDate,
            Notes = request.Notes,
            LineItems = request.LineItems.Select(x =>
                new CreateLineItemRequest
                {
                    Description = x.Description,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice
                }).ToList()
        };
    }

    public static SubmitInvoiceResponse ToApi(
        InvoiceResponse response)
    {
        return new SubmitInvoiceResponse
        {
            InvoiceId = response.Id,
            Status = response.Status,
            Message = response.Message
        };
    }
}