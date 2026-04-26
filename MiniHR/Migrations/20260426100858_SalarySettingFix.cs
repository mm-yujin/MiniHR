using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniHR.Migrations
{
    /// <inheritdoc />
    public partial class SalarySettingFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IncomeTax",
                table: "SalarySettings",
                newName: "TaxRate2");

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyEmploymentInsuranceRate",
                table: "SalarySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IncomeTaxRate",
                table: "SalarySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IndustrialAccidentInsuranceRate",
                table: "SalarySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ProgressiveDeduction2",
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
                name: "CompanyEmploymentInsurance",
                table: "SalaryLogs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyHealthInsurance",
                table: "SalaryLogs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyLongTermCare",
                table: "SalaryLogs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyNationalPension",
                table: "SalaryLogs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EmploymentInsurance",
                table: "SalaryLogs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IndustrialAccidentInsurance",
                table: "SalaryLogs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LongTermCare",
                table: "SalaryLogs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyEmploymentInsuranceRate",
                table: "SalarySettings");

            migrationBuilder.DropColumn(
                name: "IncomeTaxRate",
                table: "SalarySettings");

            migrationBuilder.DropColumn(
                name: "IndustrialAccidentInsuranceRate",
                table: "SalarySettings");

            migrationBuilder.DropColumn(
                name: "ProgressiveDeduction2",
                table: "SalarySettings");

            migrationBuilder.DropColumn(
                name: "TaxBracket2",
                table: "SalarySettings");

            migrationBuilder.DropColumn(
                name: "CompanyEmploymentInsurance",
                table: "SalaryLogs");

            migrationBuilder.DropColumn(
                name: "CompanyHealthInsurance",
                table: "SalaryLogs");

            migrationBuilder.DropColumn(
                name: "CompanyLongTermCare",
                table: "SalaryLogs");

            migrationBuilder.DropColumn(
                name: "CompanyNationalPension",
                table: "SalaryLogs");

            migrationBuilder.DropColumn(
                name: "EmploymentInsurance",
                table: "SalaryLogs");

            migrationBuilder.DropColumn(
                name: "IndustrialAccidentInsurance",
                table: "SalaryLogs");

            migrationBuilder.DropColumn(
                name: "LongTermCare",
                table: "SalaryLogs");

            migrationBuilder.RenameColumn(
                name: "TaxRate2",
                table: "SalarySettings",
                newName: "IncomeTax");
        }
    }
}
