using ApprovalService.Domain.Entities;

namespace ApprovalService.Domain.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishPaymentRequestedAsync(
            InvoiceApproval invoice,
            DateTime requestedAt);
    }
}
