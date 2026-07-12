using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public record InvoiceHumanReviewEvent(Guid InvoiceId, string TrackingId, string Reason);
}
