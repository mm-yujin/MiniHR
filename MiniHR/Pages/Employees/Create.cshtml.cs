using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniHR.Models;

namespace MiniHR.Pages.Employees
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            Employee = new Employee
            {
                Department = "0000부",
                HireDate = today,
                AnnualSalary = 0,
                Role = Employee.RoleType.User,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Employee.EmployeeNumber");
            ModelState.Remove("Employee.Password");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var hireDate = Employee.HireDate;

            int realYear = hireDate.Year;
            int realMonth = hireDate.Month;

            int thisYearEmployeeCount = await _context.Employees.CountAsync(x => x.HireDate.Year == realYear);
            int nextSequnceNumber = thisYearEmployeeCount + 1;

            Employee.EmployeeNumber = $"{realYear.ToString().Substring(2)}{realMonth.ToString("D2")}{nextSequnceNumber.ToString("D4")}";
            Employee.Password = Employee.EmployeeNumber;

            if (Employee.EmployeeNumber == null ||  Employee.EmployeeNumber.Length == 0)
                return Page();

            bool isDuplicate = await _context.Employees.AnyAsync(e => e.EmployeeNumber == Employee.EmployeeNumber);
            if (isDuplicate)
            {
                ModelState.AddModelError("Employee.EmployeeNumber", "이미 존재하는 사번입니다. 잠시 후 다시 시도해주세요.");
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
