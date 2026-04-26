using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniHR.Models;

namespace MiniHR.Pages.Salary
{
    public class SalaryDetailModel : PageModel
    {
        private readonly AppDbContext _context;
        public SalaryDetailModel(AppDbContext context) => _context = context;

        public SalaryLog? SalaryLog { get; set; }

        public async Task<IActionResult> OnGetAsync(string empNo, int yearMonth)
        {
            if (_context.SalaryLogs == null)
                return NotFound();

            if (!User.IsInRole(Employee.RoleType.Admin.ToString()) && User.Identity?.Name != empNo)
            {
                return Forbid();
            }

            SalaryLog = await _context.SalaryLogs
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.EmployeeNumber == empNo && m.YearMonth == yearMonth);

            if (SalaryLog == null) return NotFound();

            return Page();
        }
    }
}