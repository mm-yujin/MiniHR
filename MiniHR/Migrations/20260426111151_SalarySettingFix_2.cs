using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniHR.Migrations
{
    /// <inheritdoc />
    public partial class SalarySettingFix_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgressiveDeduction2",
                table: "SalarySettings");

            migrationBuilder.DropColumn(
                name: "TaxBracket1",
                table: "SalarySettings");

            migrationBuilder.DropColumn(
                name: "TaxBracket2",
                table: "SalarySettings");

            migrationBuilder.DropColumn(
                name: "TaxRate1",
                table: "SalarySettings");

            migrationBuilder.DropColumn(
                name: "TaxRate2",
                table: "SalarySettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ProgressiveDeduction2",
                table: "SalarySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxBracket1",
                table: "SalarySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxBracket2",
                table: "SalarySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate1",
                table: "SalarySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate2",
                table: "SalarySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
