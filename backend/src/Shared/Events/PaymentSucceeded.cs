using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public class PaymentSucceeded
    {
        public Guid InvoiceId { get; init; }

        public string TrackingId { get; init; } = string.Empty;

        public DateTime PaidAt { get; init; } = DateTime.UtcNow;
    }
}
