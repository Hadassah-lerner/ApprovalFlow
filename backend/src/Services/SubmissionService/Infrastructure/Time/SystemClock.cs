using SubmissionService.Application.Common.Abstractions;

namespace SubmissionService.Infrastructure.Time
{
    public class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}