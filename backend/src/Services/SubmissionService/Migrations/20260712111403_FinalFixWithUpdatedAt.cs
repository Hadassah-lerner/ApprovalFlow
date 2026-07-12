using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubmissionService.Migrations
{
    /// <inheritdoc />
    public partial class FinalFixWithUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Invoices");
        }
    }
}
