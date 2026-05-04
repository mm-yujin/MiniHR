using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniHR.Models;

namespace MiniHR.Pages.Leave
{
    [Authorize]
    public class ApplyModel : PageModel
    {
        private readonly AppDbContext _context;
        public ApplyModel(AppDbContext context) => _context = context;

        [BindProperty]
        public LeaveLog LeaveLog { get; set; } = default!;

        public decimal TotalLeave { get; set; }
        public decimal UsedLeave { get; set; }
        public decimal RemainingLeave => TotalLeave - UsedLeave;

        public async Task<IActionResult> OnGetAsync()
        {
            var empNo = User.Identity?.Name;
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeNumber == empNo);

            if (employee == null) return NotFound();

            TotalLeave = employee.GetTotalAnnualLeave(DateTime.Today);
            UsedLeave = await _context.LeaveLogs
                .Where(l => l.EmployeeNumber == empNo)
                .SumAsync(l => l.UsedDays);

            LeaveLog = new LeaveLog
            {
                EmployeeNumber = empNo!,
                LeaveDate = DateOnly.FromDateTime(DateTime.Today),
                Type = LeaveLog.LeaveType.Annual,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var empNo = User.Identity?.Name;

            LeaveLog.EmployeeNumber = empNo!; //연차 신청은 일단 자기 사번으로만(자기거만) 하게 해 둔다.

            LeaveLog.UsedDays = LeaveLog.Type == LeaveLog.LeaveType.Annual ? 1.0m : 0.5m;

            ModelState.Remove("LeaveLog.EmployeeNumber");
            if (!ModelState.IsValid) return Page();

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNumber == empNo);
            if (employee == null) return NotFound();

            var totalLeave = employee.GetTotalAnnualLeave(DateTime.Today);
            var usedLeave = await _context.LeaveLogs
                .Where(l => l.EmployeeNumber == empNo)
                .SumAsync(l => l.UsedDays);

            if (usedLeave + LeaveLog.UsedDays > totalLeave)
            {
                ModelState.AddModelError("", "잔여 연차가 부족합니다.");
                TotalLeave = totalLeave;
                UsedLeave = usedLeave;
                return Page();
            }

            bool isDuplicate = await _context.LeaveLogs
                .AnyAsync(l => l.EmployeeNumber == empNo && l.LeaveDate == LeaveLog.LeaveDate);
            if (isDuplicate)
            {
                ModelState.AddModelError("", "해당 날짜에 이미 신청된 연차가 있습니다.");
                TotalLeave = totalLeave;
                UsedLeave = usedLeave;
                return Page();
            }

            LeaveLog.CreatedAt = DateTime.Now;
            _context.LeaveLogs.Add(LeaveLog);
            await _context.SaveChangesAsync();

            return RedirectToPage("./MyLeave");
        }
    }
}