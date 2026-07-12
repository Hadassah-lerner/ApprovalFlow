using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishPaymentSucceededAsync(Payment payment);

        Task PublishPaymentFailedAsync(
            Payment payment,
            string reason);
    }
}