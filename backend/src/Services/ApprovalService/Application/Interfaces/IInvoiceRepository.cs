using ApprovalService.Domain.Entities;

namespace ApprovalService.Application.Interfaces;

public interface IInvoiceRepository
{
    Task AddAsync(InvoiceApproval invoice);

    Task<InvoiceApproval?> GetByIdAsync(Guid id);

    Task<InvoiceApproval?> GetTrackedByIdAsync(Guid id);

    Task SaveChangesAsync();

    Task<IEnumerable<InvoiceApproval>> GetByStatusAsync(string status);
}