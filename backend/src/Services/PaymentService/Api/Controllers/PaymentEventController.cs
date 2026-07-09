using Dapr;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Features.PaymentProcessing;
using Shared.Events;

namespace PaymentService.Api.Controllers;

[ApiController]
[Route("api/events")]
public class PaymentEventController : ControllerBase
{
    private readonly PaymentProcessingService _paymentProcessingService;
    private readonly ILogger<PaymentEventController> _logger;

    public PaymentEventController(
        PaymentProcessingService paymentProcessingService,
        ILogger<PaymentEventController> logger)
    {
        _paymentProcessingService = paymentProcessingService;
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

        return Ok();
    }
}