using ApprovalService.Domain.Interfaces;
using ApprovalService.Infrastructure.Persistence;
using ApprovalService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApprovalDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers().AddDapr();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var ollamaUrl =
    builder.Configuration["Ollama:BaseUrl"];

builder.Services.AddHttpClient<
    IOllamaClassifierService,
    OllamaClassifierService>(client =>
    {
        client.BaseAddress = new Uri(ollamaUrl!);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

var app = builder.Build();

app.UseCloudEvents();

app.UseAuthorization();

app.MapControllers();
app.MapSubscribeHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();