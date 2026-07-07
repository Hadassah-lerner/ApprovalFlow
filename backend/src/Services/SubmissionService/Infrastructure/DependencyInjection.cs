using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubmissionService.Application.Features.InvoiceSubmission;
using SubmissionService.Domain.Interfaces;
using SubmissionService.Infrastructure.Messaging;
using SubmissionService.Infrastructure.Persistence;
using SubmissionService.Infrastructure.Repositories;


namespace SubmissionService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SubmissionDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        services.AddScoped<IEventPublisher, DaprEventPublisher>();

        services.AddScoped<InvoiceSubmissionService>();

        return services;
    }
}
