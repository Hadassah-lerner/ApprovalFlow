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

app.UseSwagger();

app.UseSwaggerUI();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SubmissionService.Infrastructure.Persistence.SubmissionDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();