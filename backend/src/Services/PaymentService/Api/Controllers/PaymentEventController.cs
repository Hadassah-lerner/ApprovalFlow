using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Features.PaymentProcessing;
using Shared.Events;

namespace PaymentService.Api.Controllers;

[ApiController]
[Route("api/events")]
public class PaymentEventController : ControllerBase
{
    private readonly PaymentProcessingService _paymentProcessingService;
    private readonly DaprClient _daprClient;
    private readonly ILogger<PaymentEventController> _logger;

    public PaymentEventController(
        PaymentProcessingService paymentProcessingService,
        DaprClient daprClient,
        ILogger<PaymentEventController> logger)
    {
        _paymentProcessingService = paymentProcessingService;
        _daprClient = daprClient;
        _logger = logger;
    }

    [Topic("pubsub", "payment-requested")]
    [HttpPost("payment-requested")]
    public async Task<IActionResult> HandlePaymentRequestedAsync(
        [FromBody] PaymentRequested request)
    {
        _logger.LogInformation(
            "Received payment request for invoice {InvoiceId}.",
            request.InvoiceId);

        await _paymentProcessingService.ProcessAsync(request);

        await _daprClient.PublishEventAsync("pubsub", "invoice-paid", new PaymentSucceeded
        {
            InvoiceId = request.InvoiceId,
            TrackingId = request.TrackingId,
            PaidAt = DateTime.UtcNow
        });

        return Ok();
    }
}