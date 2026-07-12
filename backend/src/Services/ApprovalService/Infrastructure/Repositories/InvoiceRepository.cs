using ApprovalService.Application.Interfaces;
using ApprovalService.Domain.Entities;
using ApprovalService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace ApprovalService.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApprovalDbContext _context;

    public InvoiceRepository(ApprovalDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(InvoiceApproval invoice)
    {
        await _context.InvoiceApprovals.AddAsync(invoice);
    }

    public async Task<InvoiceApproval?> GetByIdAsync(Guid id)
    {
        return await _context.InvoiceApprovals
            .Include(x => x.LineItems)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<InvoiceApproval?> GetTrackedByIdAsync(Guid id)
    {
        return await _context.InvoiceApprovals
            .Include(i => i.LineItems)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<InvoiceApproval>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<ApprovalStatus>(status, true, out var parsedStatus))
        {
            return Enumerable.Empty<InvoiceApproval>();
        }
        return await _context.InvoiceApprovals
            .Include(x => x.LineItems)
            .Where(x => x.ApprovalStatus == parsedStatus) 
            .ToListAsync();
    }
}