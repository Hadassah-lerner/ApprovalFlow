using Microsoft.EntityFrameworkCore;
using SubmissionService.Domain.Entities;
using SubmissionService.Domain.Interfaces;
using SubmissionService.Infrastructure.Persistence;

namespace SubmissionService.Infrastructure.Repositories
{

    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly SubmissionDbContext _context;

        public InvoiceRepository(SubmissionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
        }

        public async Task<Invoice?> GetByIdAsync(Guid id)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Include(i => i.LineItems)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .AsNoTracking()
                .Include(i => i.LineItems)
                .ToListAsync();
        }

        public void Update(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
        }

        public async Task<bool> ExistsAsync(string vendor, string invoiceNumber, decimal total)
        {
            return await _context.Invoices.AnyAsync(i =>
                i.Vendor == vendor &&
                i.InvoiceNumber == invoiceNumber &&
                i.Total == total);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}