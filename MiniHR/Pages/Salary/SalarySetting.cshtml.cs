using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniHR.Models;

namespace MiniHR.Pages.Salary
{
    [Authorize(Roles = "Admin")]
    public class SalarySettingModel : PageModel
    {
        private readonly AppDbContext _context;
        public SalarySettingModel(AppDbContext context) => _context = context;

        [BindProperty]
        public SalarySetting Setting { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Setting = await _context.SalarySettings.FirstOrDefaultAsync() ?? new SalarySetting(); 
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Setting.Id == 0) _context.SalarySettings.Add(Setting);
            else _context.Update(Setting);

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
