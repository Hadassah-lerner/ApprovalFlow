using System;

/// <summary>
/// Summary description for Class1
/// </summary>
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
            Service = "Submission Service",
            Status = "Healthy"
        });
    }
}