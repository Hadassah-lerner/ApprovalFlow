using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubmissionService.Migrations
{
    /// <inheritdoc />
    public partial class AddLineItemsToSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackingId",
                table: "Invoices",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TrackingId",
                table: "Invoices",
                column: "TrackingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Vendor_InvoiceNumber_Total",
                table: "Invoices",
                columns: new[] { "Vendor", "InvoiceNumber", "Total" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_TrackingId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Vendor_InvoiceNumber_Total",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TrackingId",
                table: "Invoices");
        }
    }
}
