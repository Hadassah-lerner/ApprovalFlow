using System;

using Microsoft.AspNetCore.Mvc;

namespace SubmissionService.Api;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Service = "Approval Service",
            Status = "Healthy"
        });
    }
}
