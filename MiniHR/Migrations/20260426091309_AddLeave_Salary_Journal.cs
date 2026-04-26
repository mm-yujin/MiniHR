using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniHR.Migrations
{
    /// <inheritdoc />
    public partial class AddLeave_Salary_Journal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MealAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NationalPension = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HealthInsurance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncomeTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryLogs_Employees_EmployeeNumber",
                        column: x => x.EmployeeNumber,
                        principalTable: "Employees",
                        principalColumn: "EmployeeNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalarySettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalPensionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HealthInsuranceRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LongTermCareRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmploymentInsuranceRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MealAllowanceLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncomeTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxBracket1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxRate1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalarySettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JournalDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalDetails_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_JournalEntryId",
                table: "JournalDetails",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryLogs_EmployeeNumber",
                table: "SalaryLogs",
                column: "EmployeeNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JournalDetails");

            migrationBuilder.DropTable(
                name: "SalaryLogs");

            migrationBuilder.DropTable(
                name: "SalarySettings");

            migrationBuilder.DropTable(
                name: "JournalEntries");
        }
    }
}
