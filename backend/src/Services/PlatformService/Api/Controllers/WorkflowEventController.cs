using Dapr;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Application.Features.WorkflowTracking;
using Shared.Events;


namespace PlatformService.Api.Controllers;

[ApiController]
[Route("api/events")]
public class WorkflowEventController : ControllerBase
{
    private readonly WorkflowTrackingService _workflowTrackingService;
    private readonly ILogger<WorkflowEventController> _logger;

    public WorkflowEventController(
        WorkflowTrackingService workflowTrackingService,
        ILogger<WorkflowEventController> logger)
    {
        _workflowTrackingService = workflowTrackingService;
        _logger = logger;
    }

    [Topic("pubsub", "InvoiceSubmittedEvent")]
    [HttpPost("invoice-submitted")]
    public async Task<IActionResult> HandleInvoiceSubmittedAsync(
        [FromBody] InvoiceSubmitted invoiceSubmitted)
    {
        _logger.LogInformation(
            "Invoice {InvoiceId} entered workflow.",
            invoiceSubmitted.InvoiceId);

        await _workflowTrackingService.HandleInvoiceSubmittedAsync(invoiceSubmitted);
        return Ok();
    }

    [Topic("pubsub", "payment-succeeded")]
    [HttpPost("payment-succeeded")]
    public async Task<IActionResult> HandlePaymentSucceededAsync(
        [FromBody] PaymentSucceeded paymentSucceeded)
    {
        _logger.LogInformation(
            "Payment succeeded for invoice {InvoiceId}.",
            paymentSucceeded.InvoiceId);

        await _workflowTrackingService.HandlePaymentSucceededAsync(paymentSucceeded);

        return Ok();
    }

    [Topic("pubsub", "payment-failed")]
    [HttpPost("payment-failed")]
    public async Task<IActionResult> HandlePaymentFailedAsync(
        [FromBody] PaymentFailed paymentFailed)
    {
        _logger.LogWarning(
            "Payment failed for invoice {InvoiceId}.",
            paymentFailed.InvoiceId);

        await _workflowTrackingService.HandlePaymentFailedAsync(paymentFailed);

        return Ok();
    }
    [Topic("pubsub", "payment-requested")]
    [HttpPost("payment-requested")]
    public async Task<IActionResult> HandlePaymentRequestedAsync(
    [FromBody] PaymentRequested paymentRequested)
    {
        _logger.LogInformation(
            "Invoice {InvoiceId} was approved. Payment requested.",
            paymentRequested.InvoiceId);

        await _workflowTrackingService.HandlePaymentRequestedAsync(paymentRequested);

        return Ok();
    }
}