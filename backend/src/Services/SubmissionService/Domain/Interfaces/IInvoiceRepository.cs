using SubmissionService.Domain.Entities;

namespace SubmissionService.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice);

        Task<Invoice?> GetByIdAsync(Guid id);

        Task<Invoice?> GetByTrackingIdAsync(string trackingId);

        Task<IEnumerable<Invoice>> GetAllAsync();

        void Update(Invoice invoice);

        Task<bool> ExistsAsync(string vendor, string invoiceNumber, decimal total);
        Task SaveChangesAsync();
    }
}
