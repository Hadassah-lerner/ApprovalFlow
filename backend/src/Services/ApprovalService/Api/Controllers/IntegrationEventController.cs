using Dapr;
using Microsoft.AspNetCore.Mvc;
using ApprovalService.Application.Features.ApproveInvoice;

namespace ApprovalService.Api.Controllers;

[ApiController]
[Route("api/invoices")]
public class ApprovalController : ControllerBase
{
    private readonly ApproveInvoiceService _approveInvoiceService;

    public ApprovalController(
        ApproveInvoiceService approveInvoiceService)
    {
        _approveInvoiceService = approveInvoiceService;
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        try
        {
            await _approveInvoiceService.ApproveAsync(id);
            return Ok();
        }
        catch (InvalidOperationException ex)
        { return NotFound(new { Message = ex.Message }); }
    }
}