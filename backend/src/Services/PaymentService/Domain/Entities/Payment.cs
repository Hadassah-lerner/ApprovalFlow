using Shared.Enums;

namespace PaymentService.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }

        public Guid InvoiceId { get; private set; }

        public string TrackingId { get; private set; } = string.Empty;

        public string Vendor { get; private set; } = string.Empty;

        public decimal Amount { get; private set; }

        public Currency Currency { get; private set; }

        public PaymentStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public Payment(
     Guid invoiceId,
     string trackingId,
     string vendor,
     decimal amount,
     Currency currency,
     DateTime createdAt)
        {
            Id = Guid.NewGuid();
            InvoiceId = invoiceId;
            TrackingId = trackingId;
            Vendor = vendor;
            Amount = amount;
            Currency = currency;
            Status = PaymentStatus.Pending;
            CreatedAt = createdAt; 
        }

        public void MarkSucceeded(DateTime completedAt)
        {
            Status = PaymentStatus.Succeeded;
            CompletedAt = completedAt;
        }

        public void MarkFailed(DateTime completedAt)
        {
            Status = PaymentStatus.Failed;
            CompletedAt = completedAt;
        }
        private Payment()
        {
        }
    }
}