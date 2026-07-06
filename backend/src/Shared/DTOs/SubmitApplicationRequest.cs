using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public record LineItemDto(
        string Description,
        int Quantity,
        decimal UnitPrice
    );

    public record SubmitApplicationRequest(
        string Id, 
        string Submitter,
        string Department,
        string Vendor,
        bool VendorKnown,
        string InvoiceNumber,
        string Currency,
        string Category,
        int? Attendees, 
        List<LineItemDto> LineItems,
        decimal TaxAmount,
        decimal Total,
        bool ReceiptPresent,
        string Date,
        string Notes
    );
}
