using PaymentService.Application.Common.Abstractions;

namespace PaymentService.Infrastructure.Time
{
    public class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}