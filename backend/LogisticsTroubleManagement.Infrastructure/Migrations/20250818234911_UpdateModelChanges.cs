using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticsTroubleManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Effectiveness");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Effectiveness",
                newName: "BeforeValue");

            migrationBuilder.AlterColumn<string>(
                name: "EffectivenessType",
                table: "Effectiveness",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Effectiveness",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AfterValue",
                table: "Effectiveness",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ImprovementRate",
                table: "Effectiveness",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfterValue",
                table: "Effectiveness");

            migrationBuilder.DropColumn(
                name: "ImprovementRate",
                table: "Effectiveness");

            migrationBuilder.RenameColumn(
                name: "BeforeValue",
                table: "Effectiveness",
                newName: "Value");

            migrationBuilder.AlterColumn<string>(
                name: "EffectivenessType",
                table: "Effectiveness",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Effectiveness",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Effectiveness",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
