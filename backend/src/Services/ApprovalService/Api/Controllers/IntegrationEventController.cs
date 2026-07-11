using Dapr;
using Microsoft.AspNetCore.Mvc;
using ApprovalService.Application.Features.ProcessInvoice;
using ApprovalService.Api.Events;
using ApprovalService.Application.Interfaces;
using ApprovalService.Domain.Entities;
using Shared.Enums;
using System;
using System.Threading.Tasks;
using Dapr.Client; 

namespace ApprovalService.Api.Controllers;

[ApiController]
[Route("api/invoices")]
public class ApprovalController : ControllerBase
{
    private readonly ProcessInvoiceService _processInvoiceService;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<ApprovalController> _logger;
    private readonly DaprClient _daprClient; 

    public ApprovalController(
        ProcessInvoiceService processInvoiceService,
        IInvoiceRepository invoiceRepository,
        ILogger<ApprovalController> logger,
        DaprClient daprClient) 
    {
        _processInvoiceService = processInvoiceService;
        _invoiceRepository = invoiceRepository;
        _logger = logger;
        _daprClient = daprClient;
    }

    [Topic("pubsub", "invoice-submitted")]
    [HttpPost("sub/invoice-submitted")]
    public async Task<IActionResult> OnInvoiceSubmitted([FromBody] InvoiceSubmittedEvent invoiceSubmittedEvent)
    {
        if (invoiceSubmittedEvent == null) return BadRequest("Event data is null");

        WorkflowContext context = await _processInvoiceService.ProcessAsync(invoiceSubmittedEvent);

        if (context.FinalDecision == "Approved")
        {
            _logger.LogInformation($"[Approval AI] Invoice {invoiceSubmittedEvent.Id} was AUTO-APPROVED! Publishing payment request...");

            var paymentRequest = new
            {
                InvoiceId = invoiceSubmittedEvent.Id,
                TrackingId = invoiceSubmittedEvent.TrackingId ?? $"TRK-{Guid.NewGuid().ToString().Substring(0, 8)}",
                Amount = invoiceSubmittedEvent.Total,
                Currency = (int)invoiceSubmittedEvent.Currency,
                Vendor = invoiceSubmittedEvent.Vendor,
                RequestedAt = DateTime.UtcNow
            };

            await _daprClient.PublishEventAsync("pubsub", "payment-requested", paymentRequest);
        }
        else
        {
            _logger.LogInformation($"[Approval AI] Invoice {invoiceSubmittedEvent.Id} requires human touch. Waiting for manual approval...");

        }

        return Ok(new
        {
            InvoiceId = invoiceSubmittedEvent.Id,
            Decision = context.FinalDecision,
            Category = context.Category
        });
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        InvoiceApproval? approvalRecord = await _invoiceRepository.GetByIdAsync(id);

        if (approvalRecord == null)
        {
            return NotFound(new { Message = $"Invoice approval record with ID {id} was not found." });
        }

        approvalRecord.ChangeStatus(ApprovalStatus.Approved);

        await _invoiceRepository.SaveChangesAsync();

        var paymentRequest = new
        {
            InvoiceId = approvalRecord.Id,
            TrackingId = approvalRecord.TrackingId ?? $"TRK-{Guid.NewGuid().ToString().Substring(0, 8)}",
            Amount = approvalRecord.Total,
            Currency = (int)approvalRecord.Currency,
            Vendor = approvalRecord.Vendor,
            RequestedAt = DateTime.UtcNow
        };
        await _daprClient.PublishEventAsync("pubsub", "payment-requested", paymentRequest);

        return Ok(new { Message = "Invoice manually approved successfully and moved to payment." });
    }
}