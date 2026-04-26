using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniHR.Migrations
{
    /// <inheritdoc />
    public partial class FixAttendanceField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Employees_EmployeeId",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Attendances",
                newName: "EmployeeNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_EmployeeId",
                table: "Attendances",
                newName: "IX_Attendances_EmployeeNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Employees_EmployeeNumber",
                table: "Attendances",
                column: "EmployeeNumber",
                principalTable: "Employees",
                principalColumn: "EmployeeNumber",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Employees_EmployeeNumber",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "EmployeeNumber",
                table: "Attendances",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_EmployeeNumber",
                table: "Attendances",
                newName: "IX_Attendances_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Employees_EmployeeId",
                table: "Attendances",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
