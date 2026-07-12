using SubmissionService.Domain.Interfaces;
using Shared.Events;
using Shared.Enums;
using System.Threading.Tasks;

namespace SubmissionService.Application.EventHandlers
{
    public class InvoiceApprovalStatusChangedEventHandler
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceApprovalStatusChangedEventHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task HandleAsync(string trackingId)
        {
            var invoice = await _invoiceRepository.GetByTrackingIdAsync(trackingId);

            if (invoice != null)
            {
                invoice.ChangeStatus(ApprovalStatus.HumanReview);

                await _invoiceRepository.SaveChangesAsync();
            }
        }
    }
}