using Dapr.Client;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using Shared.Events;
using PaymentService.Application.Common.Abstractions;

namespace PaymentService.Infrastructure.Messaging
{
    public class DaprEventPublisher : IEventPublisher
    {
        private readonly DaprClient _daprClient;
        private readonly IClock _clock;

        public DaprEventPublisher(DaprClient daprClient, IClock clock)
        {
            _daprClient = daprClient;
            _clock = clock;
        }

        public async Task PublishPaymentSucceededAsync(Payment payment)
        {
            var paymentSucceeded = new PaymentSucceeded
            {
                InvoiceId = payment.InvoiceId,
                TrackingId = payment.TrackingId,
                PaidAt = payment.CompletedAt ?? _clock.UtcNow
            };

            await _daprClient.PublishEventAsync(
                "pubsub",
                "payment-succeeded",
                paymentSucceeded);
        }

        public async Task PublishPaymentFailedAsync(
            Payment payment,
            string reason)
        {
            var paymentFailed = new PaymentFailed
            {
                InvoiceId = payment.InvoiceId,
                TrackingId = payment.TrackingId,
                Reason = reason
            };

            await _daprClient.PublishEventAsync(
                "pubsub",
                "payment-failed",
                paymentFailed);
        }
    }
}