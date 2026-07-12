using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);

        Task<Payment?> GetByIdAsync(Guid id);

        Task UpdateAsync(Payment payment);

        Task SaveChangesAsync();
    }
}