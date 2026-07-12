using ApprovalService.Api.Events;
using ApprovalService.Application.Features.GetPendingInvoices;
using ApprovalService.Application.Features.ProcessInvoice;
using ApprovalService.Application.Interfaces;
using ApprovalService.Domain.Entities;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Events;
using System;
using System.Threading.Tasks;

namespace ApprovalService.Api.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class ApprovalController : ControllerBase
    {
        private readonly ProcessInvoiceService _processInvoiceService;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<ApprovalController> _logger;
        private readonly DaprClient _daprClient;
        private readonly InvoiceApprovalService _approvalService;

        public ApprovalController(
            ProcessInvoiceService processInvoiceService,
            IInvoiceRepository invoiceRepository,
            ILogger<ApprovalController> logger,
            DaprClient daprClient,
            InvoiceApprovalService approvalService)
        {
            _processInvoiceService = processInvoiceService;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _daprClient = daprClient;
            _approvalService = approvalService;
        }

        [Topic("pubsub", "invoice-submitted")]
        [HttpPost("sub/invoice-submitted")]
        public async Task<IActionResult> OnInvoiceSubmitted([FromBody] InvoiceSubmittedEvent invoiceSubmittedEvent)
        {
            if (invoiceSubmittedEvent == null) return BadRequest("Event data is null");

            _logger.LogInformation($"Processing invoice submission for TrackingId: {invoiceSubmittedEvent.TrackingId}");

            WorkflowContext context = await _processInvoiceService.ProcessAsync(invoiceSubmittedEvent);

            if (context.FinalDecision == "Approved" && invoiceSubmittedEvent.Total <= 250.00m && invoiceSubmittedEvent.VendorKnown)
            {
                await _daprClient.PublishEventAsync("pubsub", "payment-requested", new PaymentRequested
                {
                    InvoiceId = invoiceSubmittedEvent.Id,
                    TrackingId = invoiceSubmittedEvent.TrackingId,
                    Vendor = invoiceSubmittedEvent.Vendor,
                    Amount = invoiceSubmittedEvent.Total,
                    Currency = invoiceSubmittedEvent.Currency
                });
            }
            else if (context.FinalDecision == "HumanReview" || context.RequiresHumanReview)
            {
                // Publish a focused human-review event so the SubmissionService can update its status.
                var reason = context.PolicyViolations != null && context.PolicyViolations.Count > 0
                    ? string.Join("; ", context.PolicyViolations)
                    : "Requires human review";

                await _daprClient.PublishEventAsync(
                    "pubsub",
                    "invoice-human-review",
                    new Shared.Events.InvoiceHumanReviewEvent(invoiceSubmittedEvent.Id, invoiceSubmittedEvent.TrackingId, reason)
                );
            }
            else
            {
                // For other non-approved decisions (e.g. Rejected) we avoid sending a human-review event.
                _logger.LogInformation($"Invoice {invoiceSubmittedEvent.Id} final decision: {context.FinalDecision}; no human-review event published.");
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

        [HttpGet]
        public async Task<IActionResult> GetByStatus([FromQuery] string status)
        {
            try
            {
                var pendingInvoices = await _approvalService.GetPendingInvoicesAsync(status);
                return Ok(pendingInvoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}