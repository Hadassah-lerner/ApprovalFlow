using Microsoft.EntityFrameworkCore;
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();

app.UseHttpsRedirection();

app.MapControllers();

app.MapSubscribeHandler();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<PaymentDbContext>();

    dbContext.Database.Migrate();
}

app.Run();