using PlatformService.Application.Common.Abstractions;
using PlatformService.Domain.Entities;
using PlatformService.Domain.Interfaces;
using Shared.Events;

namespace PlatformService.Application.Features.WorkflowTracking
{ 
public class WorkflowTrackingService
{
    private readonly IWorkflowRepository _repository;
    private readonly IClock _clock;

    public WorkflowTrackingService(
        IWorkflowRepository repository,
        IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task HandleInvoiceSubmittedAsync(
        InvoiceSubmitted ev)
    {
        var workflow = new WorkflowState(
            ev.InvoiceId,
            ev.TrackingId,
            _clock.UtcNow);

        await _repository.AddAsync(workflow);
        await _repository.SaveChangesAsync();
    }

    public async Task HandlePaymentSucceededAsync(
        PaymentSucceeded ev)
    {
        var workflow =
            await _repository.GetByInvoiceIdAsync(ev.InvoiceId);

        if (workflow is null)
            return;

        workflow.MarkPaid(ev.PaidAt);

        await _repository.SaveChangesAsync();
    }

    public async Task HandlePaymentFailedAsync(
        PaymentFailed ev)
    {
        var workflow =
            await _repository.GetByInvoiceIdAsync(ev.InvoiceId);

        if (workflow is null)
            return;

        workflow.MarkPaymentFailed(
        _clock.UtcNow,
        ev.Reason);

        await _repository.SaveChangesAsync();
    }

    public async Task HandlePaymentRequestedAsync(PaymentRequested ev)
    {
        var workflow = await _repository.GetByInvoiceIdAsync(ev.InvoiceId);

        if (workflow is null)
            return;

        workflow.MarkPaymentRequested(_clock.UtcNow);

        await _repository.SaveChangesAsync();
    }
}
}