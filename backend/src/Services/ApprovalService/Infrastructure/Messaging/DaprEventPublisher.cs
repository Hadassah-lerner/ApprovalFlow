using ApprovalService.Domain.Entities;
using ApprovalService.Domain.Interfaces;
using Dapr.Client;
using Shared.Events;

namespace ApprovalService.Infrastructure.Messaging;

public class DaprEventPublisher : IEventPublisher
{
    private readonly DaprClient _daprClient;

    public DaprEventPublisher(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task PublishPaymentRequestedAsync(
        InvoiceApproval invoice,
        DateTime requestedAt)
    {
        var paymentRequested = new PaymentRequested
        {
            InvoiceId = invoice.Id,
            TrackingId = invoice.TrackingId,
            Vendor = invoice.Vendor,
            Amount = invoice.Total,
            Currency = invoice.Currency,
            RequestedAt = requestedAt
        };

        await _daprClient.PublishEventAsync(
            "pubsub",
            "payment-requested",
            paymentRequested);
    }
}