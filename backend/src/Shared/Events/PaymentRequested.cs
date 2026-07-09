using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public class PaymentRequested
    {
        public Guid InvoiceId { get; init; }

        public string TrackingId { get; init; } = string.Empty;

        public decimal Amount { get; init; }

        public Currency Currency { get; init; }

        public string Vendor { get; init; } = string.Empty;

        public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
    }
}
