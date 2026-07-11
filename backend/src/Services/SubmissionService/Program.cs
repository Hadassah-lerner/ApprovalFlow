using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SubmissionService.Application.Common.Abstractions;
using SubmissionService.Infrastructure;
using SubmissionService.Infrastructure.Time;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddDaprClient();

builder.Services.AddSingleton<IClock, SystemClock>();

var app = builder.Build();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = "/submission" } };
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/submission/swagger/v1/swagger.json", "Submission API V1");
    c.RoutePrefix = "submission/swagger";
});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SubmissionService.Infrastructure.Persistence.SubmissionDbContext>();
    dbContext.Database.Migrate();
}

app.Run();