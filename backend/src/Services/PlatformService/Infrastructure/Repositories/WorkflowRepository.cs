using Microsoft.EntityFrameworkCore;
using PlatformService.Domain.Entities;
using PlatformService.Domain.Interfaces;
using PlatformService.Infrastructure.Persistence;

namespace PlatformService.Infrastructure.Repositories;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly PlatformDbContext _context;

    public WorkflowRepository(PlatformDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WorkflowState workflowState)
    {
        await _context.WorkflowStates.AddAsync(workflowState);
    }

    public async Task<WorkflowState?> GetByInvoiceIdAsync(Guid invoiceId)
    {
        return await _context.WorkflowStates
            .FirstOrDefaultAsync(x => x.InvoiceId == invoiceId);
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}