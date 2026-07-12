using Dapr.Client;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using Shared.Events;
using PaymentService.Application.Common.Abstractions;
using System;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.Messaging
{
    public class DaprEventPublisher : IEventPublisher
    {
        private readonly DaprClient _daprClient;
        private readonly IClock _clock;
        private readonly ILogger<DaprEventPublisher> _logger;

        public DaprEventPublisher(DaprClient daprClient, IClock clock, ILogger<DaprEventPublisher> logger)
        {
            _daprClient = daprClient;
            _clock = clock;
            _logger = logger;
        }

        public async Task PublishPaymentSucceededAsync(Payment payment)
        {
            var paymentSucceeded = new PaymentSucceeded
            {
                InvoiceId = payment.InvoiceId,
                TrackingId = payment.TrackingId,
                PaidAt = payment.CompletedAt ?? _clock.UtcNow
            };

            try
            {
                await _daprClient.PublishEventAsync(
                    "pubsub",
                    "payment-succeeded",
                    paymentSucceeded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Isolated Test Mode] Dapr Succeeded Publish skipped/failed: {ex.Message}");
            }
        }

        public async Task PublishPaymentFailedAsync(Payment payment, string reason)
        {
            var paymentFailed = new PaymentFailed
            {
                InvoiceId = payment.InvoiceId,
                TrackingId = payment.TrackingId,
                Reason = reason
            };

            try
            {
                await _daprClient.PublishEventAsync(
                    "pubsub",
                    "payment-failed",
                    paymentFailed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Isolated Test Mode] Dapr Failed Publish skipped/failed: {ex.Message}");
            }
        }
    }
}