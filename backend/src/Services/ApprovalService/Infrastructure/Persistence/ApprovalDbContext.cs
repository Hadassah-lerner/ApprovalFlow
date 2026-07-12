using ApprovalService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ApprovalService.Infrastructure.Persistence
{
    public class ApprovalDbContext : DbContext
    {
        public ApprovalDbContext(DbContextOptions<ApprovalDbContext> options) : base(options)
        {
        }

        public DbSet<InvoiceApproval> InvoiceApprovals =>
            Set<InvoiceApproval>();

        public DbSet<LineItemApproval> LineItemApprovals =>
            Set<LineItemApproval>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InvoiceApproval>(entity =>
            {
                entity.ToTable("InvoiceApprovals");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TrackingId).HasMaxLength(50).IsRequired();
                entity.Property(e => e.InvoiceNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Submitter).HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Vendor).HasMaxLength(100);

                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Category).HasConversion<string>().HasMaxLength(50);
                entity.Property(e => e.Currency).HasConversion<string>().HasMaxLength(10);
                entity.Property(e => e.ApprovalStatus).HasConversion<string>().HasMaxLength(20);

                entity.Property(e => e.AiUrgencyLevel).HasMaxLength(20);
                entity.Property(e => e.AiSuggestedCategory).HasMaxLength(50);

                entity.HasMany(e => e.LineItems)
                      .WithOne()
                      .HasForeignKey("InvoiceApprovalId")
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<LineItemApproval>(entity =>
            {
                entity.ToTable("LineItemApprovals");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).HasMaxLength(250).IsRequired();
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            });
        }
    }
    public class ApprovalDbContextFactory : IDesignTimeDbContextFactory<ApprovalDbContext>
    {
        public ApprovalDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApprovalDbContext>();

            var connectionString = "Host=localhost;Port=5432;Database=approval_db;Username=postgres;Password=password";

            optionsBuilder.UseNpgsql(connectionString);

            return new ApprovalDbContext(optionsBuilder.Options);
        }
    }
}