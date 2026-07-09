using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Application.Features.WorkflowTracking;
using PlatformService.Domain.Interfaces;
using PlatformService.Infrastructure.Persistence;
using PlatformService.Infrastructure.Repositories;

namespace PlatformService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PlatformDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IWorkflowRepository, WorkflowRepository>();

        services.AddScoped<WorkflowTrackingService>();

        return services;
    }
}