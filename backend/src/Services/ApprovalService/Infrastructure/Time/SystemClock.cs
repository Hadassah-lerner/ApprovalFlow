using ApprovalService.Application.Common.Abstractions;

namespace ApprovalService.Infrastructure.Time
{
    public class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}