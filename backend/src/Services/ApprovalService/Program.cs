using ApprovalService.Infrastructure;
using ApprovalService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddDapr();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = "/approval" } };
        });
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/approval/swagger/v1/swagger.json", "Approval API V1");
        c.RoutePrefix = "approval/swagger";
    });
}

app.UseCloudEvents();

app.UseAuthorization();

app.MapControllers();

app.MapSubscribeHandler();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApprovalDbContext>();

    dbContext.Database.Migrate();
}

app.Run();