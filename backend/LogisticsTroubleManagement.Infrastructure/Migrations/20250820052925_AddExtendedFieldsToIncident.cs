using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticsTroubleManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExtendedFieldsToIncident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cause",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DefectiveItems",
                table: "Incidents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EffectivenessComment",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectivenessDate",
                table: "Incidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncidentDetails",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OccurrenceDate",
                table: "Incidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OccurrenceLocation",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreventionMeasures",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalShipments",
                table: "Incidents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cause",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "DefectiveItems",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "EffectivenessComment",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "EffectivenessDate",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "IncidentDetails",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "OccurrenceDate",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "OccurrenceLocation",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "PreventionMeasures",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "TotalShipments",
                table: "Incidents");
        }
    }
}
