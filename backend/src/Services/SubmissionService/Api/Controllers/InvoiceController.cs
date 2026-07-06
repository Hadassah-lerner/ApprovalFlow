using Microsoft.AspNetCore.Mvc;
using SubmissionService.Api.Mapping;
using SubmissionService.Api.Requests;
using SubmissionService.Application.Features.InvoiceSubmission;
using SubmissionService.Application.Features.InvoiceSubmission.DTOs;

namespace SubmissionService.Api.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoiceController : ControllerBase
{
    private readonly InvoiceSubmissionService _submissionService;

    public InvoiceController(InvoiceSubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitInvoice(SubmitInvoiceRequest request) // <-- קבלה של SubmitInvoiceRequest של ה-Api
    {
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

        return Ok(invoice);
    }

    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var invoice = await _submissionService.GetByIdAsync(id);

        if (invoice == null)
            return NotFound();

        return Ok(new
        {
            InvoiceId = invoice.Id,
            Status = invoice.ApprovalStatus.ToString()
        });
    }
}