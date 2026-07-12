using Dapr.Client;
using Shared.Events;
using SubmissionService.Domain.Entities;
using SubmissionService.Domain.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SubmissionService.Infrastructure.Messaging
{
    public class DaprEventPublisher : IEventPublisher
    {
        private readonly DaprClient _daprClient;

        public DaprEventPublisher(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public async Task PublishInvoiceSubmittedAsync(Invoice invoice)
        {
            var invoiceSubmittedEvent = new InvoiceSubmitted
            {
                InvoiceId = invoice.Id,
                TrackingId = invoice.TrackingId,
                Submitter = invoice.Submitter,
                Department = invoice.Department,
                Vendor = invoice.Vendor,
                VendorKnown = invoice.VendorKnown,
                InvoiceNumber = invoice.InvoiceNumber,
                Category = invoice.Category,
                Currency = invoice.Currency,
                Total = invoice.Total,
                TaxAmount = invoice.TaxAmount,
                ReceiptPresent = invoice.ReceiptPresent,
                InvoiceDate = invoice.InvoiceDate,
                Notes = invoice.Notes,

                LineItems = invoice.LineItems?.Select(item => new SharedLineItemDto
                {
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList() ?? new System.Collections.Generic.List<SharedLineItemDto>()
            };

            await _daprClient.PublishEventAsync("pubsub", "invoice-submitted", invoiceSubmittedEvent);
        }
    }
}