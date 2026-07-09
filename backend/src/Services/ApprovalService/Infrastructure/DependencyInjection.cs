using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ApproveInvoice;
using ApprovalService.Application.Interfaces;
using ApprovalService.Domain.Interfaces;
using ApprovalService.Infrastructure.Messaging;
using ApprovalService.Infrastructure.Persistence;
using ApprovalService.Infrastructure.Repositories;
using ApprovalService.Infrastructure.Services;
using ApprovalService.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApprovalService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApprovalDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        services.AddScoped<IEventPublisher, DaprEventPublisher>();

        services.AddScoped<ApproveInvoiceService>();

        services.AddSingleton<IClock, SystemClock>();

        services.AddDaprClient();

        var ollamaUrl = configuration["Ollama:BaseUrl"];

        services.AddHttpClient<
            IOllamaClassifierService,
            OllamaClassifierService>(client =>
            {
                client.BaseAddress = new Uri(ollamaUrl!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

        return services;
    }
}