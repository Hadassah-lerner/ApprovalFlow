using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public class PaymentFailed
    {
        public Guid InvoiceId { get; init; }

        public string TrackingId { get; init; } = string.Empty;

        public string Reason { get; init; } = string.Empty;

        public DateTime FailedAt { get; init; } = DateTime.UtcNow;
    }
}
