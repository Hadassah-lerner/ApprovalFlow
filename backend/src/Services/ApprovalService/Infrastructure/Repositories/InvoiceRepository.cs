using Microsoft.EntityFrameworkCore;
using ApprovalService.Application.Interfaces;
using ApprovalService.Domain.Entities;
using ApprovalService.Infrastructure.Persistence;

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

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}