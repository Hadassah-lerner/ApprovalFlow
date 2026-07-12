using Microsoft.EntityFrameworkCore;
using PlatformService.Application.Common.Abstractions;
using PlatformService.Infrastructure;
using PlatformService.Infrastructure.Persistence;
using PlatformService.Infrastructure.Time;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddDapr();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton<IClock, SystemClock>();

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
        .GetRequiredService<PlatformDbContext>();

    dbContext.Database.Migrate();
}

app.Run();