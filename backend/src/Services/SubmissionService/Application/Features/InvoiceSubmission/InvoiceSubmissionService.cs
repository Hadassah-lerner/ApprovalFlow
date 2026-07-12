using Shared.Enums;
using SubmissionService.Application.Common.Abstractions;
using SubmissionService.Application.Features.InvoiceSubmission.DTOs;
using SubmissionService.Application.Features.InvoiceSubmission.Mapping;
using SubmissionService.Application.Features.InvoiceSubmission.Validators;
using SubmissionService.Domain.Entities;
using SubmissionService.Domain.Interfaces;

namespace SubmissionService.Application.Features.InvoiceSubmission;

public class InvoiceSubmissionService
{
    private readonly IInvoiceRepository _repository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IClock _clock;

    public InvoiceSubmissionService(
        IInvoiceRepository repository,
        IEventPublisher eventPublisher,
        IClock clock)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
        _clock = clock;
    }

    public async Task<InvoiceResponse> SubmitAsync(CreateInvoiceRequest request)
    {
        var validation = SubmitInvoiceValidator.Validate(request, _clock.UtcNow);

        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validation.Errors));
        }

        var invoice = InvoiceMapper.ToEntity(request);
        invoice.ChangeStatus(ApprovalStatus.Submitted);

        var exists = await _repository.ExistsAsync(
            invoice.Vendor,
            invoice.InvoiceNumber,
            invoice.Total);

        if (exists)
            throw new InvalidOperationException("Invoice already exists.");

        await _repository.AddAsync(invoice);
        await _repository.SaveChangesAsync();

        await _eventPublisher.PublishInvoiceSubmittedAsync(invoice);

        return new InvoiceResponse
        {
            Id = invoice.Id,
            Status = invoice.ApprovalStatus.ToString(),
            Message = "Invoice submitted successfully."
        };
    }

    public Task<Invoice?> GetByIdAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _repository.SaveChangesAsync();
    }
}
