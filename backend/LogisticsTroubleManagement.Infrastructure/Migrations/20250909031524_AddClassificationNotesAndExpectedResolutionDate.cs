using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticsTroubleManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClassificationNotesAndExpectedResolutionDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassificationNotes",
                table: "Incidents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedResolutionDate",
                table: "Incidents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassificationNotes",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ExpectedResolutionDate",
                table: "Incidents");
        }
    }
}
