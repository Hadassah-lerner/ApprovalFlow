using PlatformService.Domain.Entities;

namespace PlatformService.Domain.Interfaces;

public interface IWorkflowRepository
{
    Task AddAsync(WorkflowState workflowState);

    Task<WorkflowState?> GetByInvoiceIdAsync(Guid invoiceId);

    Task SaveChangesAsync();
}