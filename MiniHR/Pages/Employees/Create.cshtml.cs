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

        public async Task<IActionResult> OnGetAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            int currentYear = today.Year;
            int currentMonth = today.Month;

            int thisYearEmployeeCount = await _context.Employees.CountAsync(x => x.HireDate.Year == currentYear);
            int nextSequnceNumber = thisYearEmployeeCount + 1;

            //사번 예) 24030001
            string newEmployeeNumber = $"{currentYear.ToString().Substring(2)}{currentMonth.ToString("D2")}{nextSequnceNumber.ToString("D4")}";

            Employee = new Employee
            {
                Department = "0000부",
                HireDate = today,
                EmployeeNumber = newEmployeeNumber,
                AnnualSalary = 0,
                Role = Employee.RoleType.User,
                Password = newEmployeeNumber, //초기 비번 = 사번
            };

            return Page();
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
