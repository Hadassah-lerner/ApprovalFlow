using ApprovalService.Domain.Entities;

namespace ApprovalService.Application.Interfaces;

public interface IInvoiceRepository
{
    Task AddAsync(InvoiceApproval invoice);

    Task<InvoiceApproval?> GetByIdAsync(Guid id);

    Task SaveChangesAsync();
}