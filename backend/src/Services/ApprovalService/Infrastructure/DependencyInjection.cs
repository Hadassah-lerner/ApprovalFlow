using ApprovalService.Application.Common.Abstractions;
using ApprovalService.Application.Features.ProcessInvoice;
using ApprovalService.Application.Helpers;
using ApprovalService.Application.Interfaces;
using ApprovalService.Application.Nodes;
using ApprovalService.Domain.Interfaces;
using ApprovalService.Infrastructure.Messaging;
using ApprovalService.Infrastructure.Persistence;
using ApprovalService.Infrastructure.Repositories;
using ApprovalService.Infrastructure.Services;
using ApprovalService.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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
        services.AddSingleton<IClock, SystemClock>();
        services.AddDaprClient();

        services.AddScoped<IPolicyLoader, PolicyLoader>();
        services.AddSingleton<PromptBuilder>();
        services.AddSingleton<JsonExtractor>();

        services.AddScoped<PreprocessNode>();
        services.AddScoped<LoadPolicyNode>();
        services.AddScoped<BuildPromptNode>();

        services.AddScoped<ClassifierNode>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("OllamaClient");
            return new ClassifierNode(
                provider.GetRequiredService<PromptBuilder>(),
                provider.GetRequiredService<JsonExtractor>(),
                client
            );
        });

        services.AddScoped<RouterNode>();
        services.AddScoped<HumanReviewNode>();
        services.AddScoped<SaveNode>();

        services.AddScoped<ProcessInvoiceService>();

        var ollamaUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434/";

        services.AddHttpClient("OllamaClient", client =>
        {
            client.BaseAddress = new Uri(ollamaUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddHttpClient<IOllamaClassifierService, OllamaClassifierService>(client =>
        {
            client.BaseAddress = new Uri(ollamaUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}