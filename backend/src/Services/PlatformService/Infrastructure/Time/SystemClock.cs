
using PlatformService.Application.Common.Abstractions;

namespace PlatformService.Infrastructure.Time
{
    public class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}