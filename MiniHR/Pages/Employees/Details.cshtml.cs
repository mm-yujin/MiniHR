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
        public Attendance TodayAttendance { get; set; } = default!;
        public int TotalAttandanceCount = 0;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            if (!User.IsInRole(Employee.RoleType.Admin.ToString()) && User.Identity?.Name != id)
            {
                return Forbid();
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.EmployeeNumber == id);
            
            if (employee == null) return NotFound();
            Employee = employee;

            if (_context.Attendances != null)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var attendance = await _context.Attendances.FirstOrDefaultAsync(a => a.EmployeeId == id && a.WorkDate == today);

                if (attendance != null)
                    TodayAttendance = attendance;

                var monthStart = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
                TotalAttandanceCount = await _context.Attendances
                    .CountAsync(a => a.EmployeeId == Employee.EmployeeNumber && a.WorkDate >= monthStart);
            }

            return Page();
        }
    }
}
