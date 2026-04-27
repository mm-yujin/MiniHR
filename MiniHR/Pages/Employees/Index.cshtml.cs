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

        public IList<Employee> EmployeeList { get;set; } = default!;
        public Dictionary<string, Attendance?> AttendanceDict { get; set; } = new Dictionary<string, Attendance?>();

        public int TotalCheckedIn { get; set; }
        public int TotalCount { get; set; }


        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; } // 검색어

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1; // 현재 페이지

        public int TotalPages { get; set; }
        public const int PageSize = 10;

        public Dictionary<string, decimal> LeaveUsageDict { get; set; } = new Dictionary<string, decimal>();


        public async Task OnGetAsync()
        {
            var employeeQuery = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                employeeQuery = employeeQuery.Where(s => s.Name.Contains(SearchString)
                                                      || s.Department.Contains(SearchString));
            }

            int totalCount = await employeeQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            EmployeeList = await employeeQuery
                .AsNoTracking()
                .OrderBy(e => e.EmployeeNumber)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var employeeIds = EmployeeList.Select(e => e.EmployeeNumber).ToList();

            LeaveUsageDict = await _context.LeaveLogs
                .AsNoTracking()
                .Where(l => employeeIds.Contains(l.EmployeeNumber))
                .GroupBy(l => l.EmployeeNumber)
                .Select(g => new {
                    EmployeeNumber = g.Key,
                    UsedDays = g.Sum(x => x.UsedDays)
                })
                .ToDictionaryAsync(x => x.EmployeeNumber, x => x.UsedDays);

            var today = DateOnly.FromDateTime(DateTime.Now);

            //오늘의 전체 Attendances. 페이지랑 상관없이 전체 출근자 확인하는데 필요하므로 미리 이걸 구한다
            var allTodayAttendances = await _context.Attendances
                .AsNoTracking()
                .Where(a => a.WorkDate == today)
                .ToListAsync();

            AttendanceDict = allTodayAttendances
                .ToDictionary(a => a.EmployeeNumber, a => (Attendance?)a);
                        
            TotalCheckedIn = allTodayAttendances.Count;

            var totalEmployeeCount = await _context.Employees.CountAsync();
            TotalCount = totalEmployeeCount;
        }

        public async Task<IActionResult> OnPostCheckInAsync(string? employeeNumber)
        {
            if (!User.IsInRole(Employee.RoleType.Admin.ToString()) && User.Identity?.Name != employeeNumber)
            {
                return RedirectToPage();
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);

            if (employeeNumber == null || employee == null) return NotFound();

            var today = DateOnly.FromDateTime(DateTime.Now);

            var attendance = await _context.Attendances.FirstOrDefaultAsync(x => x.EmployeeNumber == employeeNumber && x.WorkDate == today);
            if (attendance != null) //오늘 출근 기록이 이미 있다면 출근 처리를 무시하고 끝내도록 함
            {
                return RedirectToPage();
            }

            //오늘은 아닌 가장 최근의 출근 기록이 퇴근 처리가 안 되어 있으면, 그거 처리 후 진행.
            var lastLog = await _context.Attendances
                .Where(x => x.EmployeeNumber == employeeNumber)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (lastLog != null && lastLog.CheckOutTime == null)
            {
                lastLog.CheckOutTime = lastLog.WorkDate.ToDateTime(new TimeOnly(23, 59, 59));
                _context.Update(lastLog);
            }

            attendance = new Attendance
            {
                EmployeeNumber = employeeNumber,
                WorkDate = today,
                CheckInTime = DateTime.Now
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCheckOutAsync(string? employeeNumber)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var attendance = await _context.Attendances
                .Where(a => a.EmployeeNumber == employeeNumber && a.CheckOutTime == null)
                .OrderByDescending(a => a.WorkDate)
                .FirstOrDefaultAsync();

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
