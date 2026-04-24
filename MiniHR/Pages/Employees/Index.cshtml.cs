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
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Employee> Employee { get;set; } = default!;
        public Dictionary<int, Attendance?> AttendanceDict { get; set; } = new Dictionary<int, Attendance?>();

        public int TotalCheckedIn { get; set; }
        public int TotalNotCheckedIn { get; set; }

        public async Task OnGetAsync()
        {
            Employee = await _context.Employees.ToListAsync();

            var today = DateOnly.FromDateTime(DateTime.Now);
            var todayAttendances = await _context.Attendances
                .Where(a => a.WorkDate == today)
                .ToListAsync();

            AttendanceDict = Employee.ToDictionary(e => e.Id, e => todayAttendances.FirstOrDefault(x => x.EmployeeId == e.Id));

            TotalCheckedIn = todayAttendances.DistinctBy(x => x.EmployeeId).Count();
            TotalNotCheckedIn = Employee.Count - TotalCheckedIn;
        }

        public async Task<IActionResult> OnPostCheckInAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            var today = DateOnly.FromDateTime(DateTime.Now);

            var attendance = await _context.Attendances.FirstOrDefaultAsync(x => x.EmployeeId == id && x.WorkDate == today);
            if (attendance != null) //오늘 출근 기록이 이미 있다면 출근 처리를 무시하고 끝내도록 함
            {
                return RedirectToPage();
            }

            //오늘은 아닌 가장 최근의 출근 기록이 퇴근 처리가 안 되어 있으면, 그거 처리 후 진행.
            var lastLog = _context.Attendances
                .OrderByDescending(x => x.Id)
                .Where(x => x.EmployeeId == id)
                .FirstOrDefault();

            if (lastLog != null && lastLog.CheckOutTime == null)
            {
                await OnPostCheckOutAsync(id);
            }

            attendance = new Attendance
            {
                EmployeeId = id,
                WorkDate = today,
                CheckInTime = DateTime.Now
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCheckOutAsync(int id)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == id && a.WorkDate == today && a.CheckOutTime == null);
            
            // 기록이 없거나 이미 퇴근 시간이 있다면 무시
            if (attendance == null || attendance.CheckOutTime != null)
            {
                return RedirectToPage();
            }

            attendance.CheckOutTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
