using ApprovalService.Domain.Interfaces;
using ApprovalService.Domain.Entities;
using ApprovalService.Infrastructure.Persistence;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ApprovalService.Api.Events;

namespace ApprovalService.Api.Controllers
{
    [ApiController]
    [Route("api/sub")]
    public class IntegrationEventController : ControllerBase
    {
        private readonly ApprovalDbContext _dbContext;
        private readonly IOllamaClassifierService _aiService;
        private readonly ILogger<IntegrationEventController> _logger;

        public IntegrationEventController(
            ApprovalDbContext dbContext,
            IOllamaClassifierService aiService,
            ILogger<IntegrationEventController> logger)
        {
            _dbContext = dbContext;
            _aiService = aiService;
            _logger = logger;
        }

        [Topic("pubsub", "InvoiceSubmittedEvent")]
        [HttpPost("invoice-submitted")]
        public async Task<IActionResult> HandleInvoiceSubmittedAsync([FromBody] InvoiceSubmittedEvent ev)
        {
            _logger.LogInformation(
                "Invoice {InvoiceId} received from Submission Service.",
                ev.Id);
            try
            {
                var approvalLineItems = ev.LineItems
                    .Select(item => new LineItemApproval(item.Description, item.Quantity, item.UnitPrice))
                    .ToList();

                var invoiceApproval = new InvoiceApproval(
                    ev.Id, 
                    ev.TrackingId,
                    ev.Submitter,
                    ev.Department,
                    ev.Vendor,
                    ev.VendorKnown,
                    ev.InvoiceNumber,
                    ev.Category,
                    ev.Currency,
                    approvalLineItems,
                    ev.TaxAmount,
                    ev.Total,
                    ev.ReceiptPresent,
                    ev.InvoiceDate,
                    ev.Notes
                );

                var itemsDescription = string.Join(", ", ev.LineItems.Select(i => $"{i.Quantity}x {i.Description}"));

                _logger.LogInformation(
                    "Sending invoice {InvoiceId} to Ollama for AI classification.",
                    ev.Id);
                var aiResult = await _aiService.ClassifyInvoiceAsync(ev.Vendor, ev.Total, itemsDescription);

                invoiceApproval.SetAiTriageResults(aiResult.UrgencyLevel, aiResult.Category, aiResult.Reasoning);
                _logger.LogInformation(
                    "AI classification completed. Urgency: {Urgency}, Suggested category: {Category}",
                    aiResult.UrgencyLevel,
                    aiResult.Category);

                _dbContext.InvoiceApprovals.Add(invoiceApproval);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "Invoice {InvoiceId} saved successfully.",
                    ev.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while processing InvoiceSubmittedEvent.");
                return Ok();
            }
        }
    }

   
}