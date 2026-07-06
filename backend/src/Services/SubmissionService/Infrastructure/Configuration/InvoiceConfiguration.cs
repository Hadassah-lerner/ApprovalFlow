using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubmissionService.Domain.Entities;

namespace SubmissionService.Infrastructure.Configuration
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TrackingId) 
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Submitter)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Department)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Vendor)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.Property(x => x.Total)
                .HasPrecision(18, 2);

            builder.Property(x => x.TaxAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.ApprovalStatus)
                .HasConversion<string>();

            builder.Property(x => x.Category)
                .HasConversion<string>();

            builder.Property(x => x.Currency)
                .HasConversion<string>();

            builder.OwnsMany(x => x.LineItems, lineItem =>
            {
                lineItem.WithOwner().HasForeignKey("InvoiceId");

                lineItem.Property<int>("Id");
                lineItem.HasKey("Id");

                lineItem.Property(x => x.Description)
                    .IsRequired()
                    .HasMaxLength(250);

                lineItem.Property(x => x.UnitPrice)
                    .HasPrecision(18, 2);
            });
        }
    }
}