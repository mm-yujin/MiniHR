using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniHR.Models;

namespace MiniHR.Pages.Employees
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public Employee Employee { get; set; } = default!;
        public IList<Attendance> Attendances { get; set; } = default!;
        public Attendance? TodayAttendance { get; set; }

        public int TotalAttandanceCount = 0;


        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public const int PageSize = 10;

        
        public decimal TotalAnnualLeave { get; set; }
        public decimal UsedAnnualLeave { get; set; }
        public decimal RemainingAnnualLeave => TotalAnnualLeave - UsedAnnualLeave;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
                return NotFound();

            if (!User.IsInRole(Employee.RoleType.Admin.ToString()) && User.Identity?.Name != id)
            {
                return Forbid();
            }

            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.EmployeeNumber == id);

            if (employee == null)
                return NotFound();

            Employee = employee;

            TotalAnnualLeave = Employee.GetTotalAnnualLeave(DateTime.Today);

            UsedAnnualLeave = await _context.LeaveLogs
                .AsNoTracking()
                .Where(l => l.EmployeeNumber == id)
                .SumAsync(l => l.UsedDays);

            if (_context.Attendances != null)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                TodayAttendance = await _context.Attendances
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.EmployeeNumber == id && a.WorkDate == today);

                var query = _context.Attendances.Where(x => x.EmployeeNumber == id); //일단은 전체 데이터로하는데 월별 데이터로 하려면 여기에 조건 추가하자.

                TotalAttandanceCount = await query.CountAsync();

                TotalPages = (int)Math.Ceiling(TotalAttandanceCount / (double)PageSize);

                Attendances = await query
                    .AsNoTracking()
                    .OrderByDescending(x => x.WorkDate)
                    .Skip((CurrentPage - 1) * PageSize) //현재보다 앞의 페이지들은 스킵하고 현재 페이지 것만 가져오도록.
                    .Take(PageSize)
                    .ToListAsync();
            }

            return Page();
        }
    }
}
