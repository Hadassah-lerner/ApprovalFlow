using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PaymentService.Infrastructure;
using PaymentService.Infrastructure.Persistence;

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
            swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = "/payment" } };
        });
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/payment/swagger/v1/swagger.json", "Payment API V1");
        c.RoutePrefix = "payment/swagger";
    });
}

app.UseCloudEvents();

// app.UseHttpsRedirection();

app.MapControllers();

app.MapSubscribeHandler();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<PaymentDbContext>();

    dbContext.Database.Migrate();
}

app.Run();