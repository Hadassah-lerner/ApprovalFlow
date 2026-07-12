using PaymentService.Application.Common.Abstractions;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using Shared.Events;
using System;
using System.Threading.Tasks;

namespace PaymentService.Application.Features.PaymentProcessing
{
    public class PaymentProcessingService
    {
        private readonly IPaymentRepository _repository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IClock _clock;

        public PaymentProcessingService(
            IPaymentRepository repository,
            IEventPublisher eventPublisher,
            IClock clock)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
            _clock = clock;
        }

        public async Task ProcessAsync(PaymentRequested request)
        {
            var payment = new Payment(
                request.InvoiceId,
                request.TrackingId,
                request.Vendor,
                request.Amount,
                request.Currency,
                _clock.UtcNow);

            await _repository.AddAsync(payment);
            await _repository.SaveChangesAsync();

            if (request.Amount > 10000)
            {
                payment.MarkFailed(_clock.UtcNow);

                await _repository.UpdateAsync(payment);
                await _repository.SaveChangesAsync();

                await _eventPublisher.PublishPaymentFailedAsync(
                    payment,
                    "Amount exceeds maximum automated limit (10,000).");

                return;
            }

            payment.MarkSucceeded(_clock.UtcNow);

            await _repository.UpdateAsync(payment);
            await _repository.SaveChangesAsync();

          
            await _eventPublisher.PublishPaymentSucceededAsync(payment);
        }
    }
}