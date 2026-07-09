using Microsoft.EntityFrameworkCore;
using PlatformService.Domain.Entities;
using PlatformService.Domain.Enums;

namespace PlatformService.Infrastructure.Persistence;

public class PlatformDbContext : DbContext
{
    public PlatformDbContext(
        DbContextOptions<PlatformDbContext> options)
        : base(options)
    {
    }

    public DbSet<WorkflowState> WorkflowStates =>
        Set<WorkflowState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkflowState>(entity =>
        {
            entity.ToTable("WorkflowStates");

            entity.HasKey(x => x.InvoiceId);

            entity.Property(x => x.TrackingId)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(x => x.CurrentStatus)
                  .HasConversion<string>()
                  .HasMaxLength(30)
                  .IsRequired();

            entity.Property(x => x.CreatedAt)
                  .IsRequired();

            entity.Property(x => x.UpdatedAt)
                  .IsRequired();

            entity.Property(x => x.FailureReason)
                  .HasMaxLength(500);
        });
    }
}