using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Events;
using SubmissionService.Api.Mapping;
using SubmissionService.Api.Requests;
using SubmissionService.Api.Responses;
using SubmissionService.Application.Features.InvoiceSubmission;
using SubmissionService.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SubmissionService.Api.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceSubmissionService _submissionService;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(InvoiceSubmissionService submissionService, IInvoiceRepository invoiceRepository, ILogger<InvoiceController> logger)
        {
            _submissionService = submissionService;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }

        [Topic("pubsub", "invoice-human-review")]
        [HttpPost("sub/invoice-human-review")]
        public async Task<IActionResult> OnInvoiceHumanReview([FromBody] InvoiceHumanReviewEvent @event)
        {
            if (@event == null)
            {
                _logger.LogWarning("Received null InvoiceHumanReviewEvent payload.");
                return BadRequest("Event data is null");
            }

            _logger.LogInformation($"[Submission Dapr] Received invoice-human-review for Invoice ID: {@event.InvoiceId}, TrackingId: {@event.TrackingId}, Reason: {@event.Reason}");

            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(@event.InvoiceId);

                if (invoice != null)
                {
                    invoice.UpdateStatus(ApprovalStatus.HumanReview);
                    _invoiceRepository.Update(invoice);
                    await _invoiceRepository.SaveChangesAsync();

                    _logger.LogInformation($"[SubmissionService] Invoice {@event.InvoiceId} status updated to HumanReview in DB.");
                    return Ok();
                }

                _logger.LogWarning($"[SubmissionService] Invoice {@event.InvoiceId} not found for HumanReview update.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[SubmissionService] Failed to update invoice status to HumanReview for ID: {@event.InvoiceId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Topic("pubsub", "payment-requested")]
        [HttpPost("sub/payment-requested")]
        public async Task<IActionResult> OnPaymentRequested([FromBody] PaymentRequested paymentEvent)
        {
            if (paymentEvent == null) return BadRequest("Event data is null");
            _logger.LogInformation($"[Submission Dapr] Received event for Invoice ID: {paymentEvent.InvoiceId}");

            try
            {
                var invoice = await _submissionService.GetByIdAsync(paymentEvent.InvoiceId);

                if (invoice != null)
                {
                    invoice.UpdateStatus(ApprovalStatus.Approved);
                    await _submissionService.SaveChangesAsync();

                    _logger.LogInformation($"[SubmissionService] Invoice {invoice.Id} status updated to Approved in DB.");
                }
                else
                {
                    _logger.LogWarning($"[SubmissionService] Invoice {paymentEvent.InvoiceId} not found for status update.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[SubmissionService] Failed to update invoice status for ID: {paymentEvent.InvoiceId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Topic("pubsub", "invoice-paid")]
        [HttpPost("sub/invoice-paid")]
        public async Task<IActionResult> OnInvoicePaid([FromBody] PaymentSucceeded paidEvent)
        {
            if (paidEvent == null) return BadRequest("Event data is null");

            _logger.LogInformation($"[SubmissionService] Received invoice-paid event for Invoice ID: {paidEvent.InvoiceId}");

            try
            {
                var invoice = await _submissionService.GetByIdAsync(paidEvent.InvoiceId);

                if (invoice != null)
                {
                    invoice.UpdateStatus(ApprovalStatus.Paid);
                    await _submissionService.SaveChangesAsync();

                    _logger.LogInformation($"[SubmissionService] SUCCESS: Invoice {invoice.Id} marked as PAID in DB.");
                }
                else
                {
                    _logger.LogWarning($"[SubmissionService] WARNING: Invoice {paidEvent.InvoiceId} not found for Paid update.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[SubmissionService] Error updating paid status for ID: {paidEvent.InvoiceId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitInvoice([FromBody] SubmitInvoiceRequest request)
        {
            if (request == null) return BadRequest("Request body is null");

            try
            {
                var appRequest = ApiMapper.ToApplication(request);
                var result = await _submissionService.SubmitAsync(appRequest);
                var apiResponse = ApiMapper.ToApi(result);

                return Ok(apiResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            var invoice = await _submissionService.GetByIdAsync(id);

            if (invoice == null)
                return NotFound();

            return Ok(ApiMapper.ToDetailsResponse(invoice));
        }

        [HttpGet("{id:guid}/status")]
        public async Task<IActionResult> GetStatus(Guid id)
        {
            var invoice = await _submissionService.GetByIdAsync(id);

            if (invoice == null)
                return NotFound();

            return Ok(new InvoiceStatusResponse
            {
                InvoiceId = invoice.Id,
                Status = invoice.ApprovalStatus.ToString()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoices([FromQuery] string? status = null)
        {
            try
            {
                var invoices = await _submissionService.GetAllAsync();

                if (invoices == null)
                {
                    return Ok(Array.Empty<InvoiceDetailsResponse>());
                }

                if (!string.IsNullOrEmpty(status))
                {
                    invoices = invoices.Where(i => i.ApprovalStatus.ToString().Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                var response = invoices.Select(invoice => ApiMapper.ToDetailsResponse(invoice)).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllInvoices");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}