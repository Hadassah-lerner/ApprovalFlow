using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Domain.Interfaces;
using ApprovalService.Application.Interfaces;
using Shared.Enums;

namespace ApprovalService.Application.Features.ApproveInvoice;

public class ApproveInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IClock _clock;

    public ApproveInvoiceService(
        IInvoiceRepository repository,
        IEventPublisher eventPublisher,
        IClock clock)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
        _clock = clock;
    }

    public async Task ApproveAsync(Guid invoiceId)
    {
        var invoice = await _repository.GetByIdAsync(invoiceId);

        if (invoice is null)
            throw new InvalidOperationException(
                $"Invoice {invoiceId} was not found.");

        invoice.ChangeStatus(ApprovalStatus.Approved);

        await _repository.SaveChangesAsync();

        await _eventPublisher.PublishPaymentRequestedAsync(
            invoice,
            _clock.UtcNow);
    }
}