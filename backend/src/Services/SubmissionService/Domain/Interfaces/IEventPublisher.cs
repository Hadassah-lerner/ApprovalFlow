using SubmissionService.Domain.Entities;

namespace SubmissionService.Domain.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishInvoiceSubmittedAsync(Invoice invoice);
    }
}
