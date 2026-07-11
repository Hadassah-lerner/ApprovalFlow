using System;

using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Api;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Service = "Payment Service",
            Status = "Healthy"
        });
    }
}
