using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniHR.Models;

namespace MiniHR.Pages.Leave
{
    [Authorize]
    public class MyLeaveModel : PageModel
    {
        private readonly AppDbContext _context;
        public MyLeaveModel(AppDbContext context) => _context = context;

        public IList<LeaveLog> LeaveLogs { get; set; } = default!;
        public decimal TotalLeave { get; set; }
        public decimal UsedLeave { get; set; }
        public decimal RemainingLeave => TotalLeave - UsedLeave;

        public async Task OnGetAsync()
        {
            var empNo = User.Identity?.Name;

            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeNumber == empNo);

            if (employee != null)
                TotalLeave = employee.GetTotalAnnualLeave(DateTime.Today);

            LeaveLogs = await _context.LeaveLogs
                .Where(l => l.EmployeeNumber == empNo)
                .OrderByDescending(l => l.LeaveDate)
                .AsNoTracking()
                .ToListAsync();

            UsedLeave = LeaveLogs.Sum(l => l.UsedDays);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var empNo = User.Identity?.Name;
            var log = await _context.LeaveLogs.FindAsync(id);

            if (log == null || log.EmployeeNumber != empNo) return Forbid();
            if (log.LeaveDate < DateOnly.FromDateTime(DateTime.Today))
            {
                TempData["Error"] = "이미 지난 연차는 취소할 수 없습니다.";
                return RedirectToPage();
            }

            _context.LeaveLogs.Remove(log);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}