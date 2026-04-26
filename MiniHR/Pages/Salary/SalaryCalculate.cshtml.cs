using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniHR.Models;
using MiniHR.Services;

namespace MiniHR.Pages.Salary
{
    [Authorize(Roles = "Admin")]
    public class SalaryCalculateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly SalaryService _salaryService;

        public SalaryCalculateModel(AppDbContext context, SalaryService salaryService)
        {
            _context = context;
            _salaryService = salaryService;
        }

        public List<Employee> TargetEmployees { get; set; } = new();
        public List<SalaryLog> CompletedSalaryLogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            int currentYearMonth = int.Parse(DateTime.Today.ToString("yyyyMM"));

            var paidEmployeeNumbers = await _context.SalaryLogs
                .Where(s => s.YearMonth == currentYearMonth)
                .Select(s => s.EmployeeNumber)
                .ToListAsync();

            TargetEmployees = await _context.Employees
                .Where(e => !paidEmployeeNumbers.Contains(e.EmployeeNumber))
                .ToListAsync();

            CompletedSalaryLogs = await _context.SalaryLogs
                .Include(s => s.Employee)
                .Where(s => s.YearMonth == currentYearMonth)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostGenerateAllAsync()
        {
            int currentYearMonth = int.Parse(DateTime.Today.ToString("yyyyMM"));
            string currentMonthDesc = $"{DateTime.Today.Year}.{DateTime.Today.Month}월 정기 급여";

            try
            {
                // 급여계산 & 전표처리 는 서비스로 빼서 처리한다
                await _salaryService.CreateMonthlySalaryProcessAsync(currentYearMonth, currentMonthDesc);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"처리 중 오류가 발생했습니다: {ex.Message}");
                return Page();
            }

            return RedirectToPage();
        }
    }
}