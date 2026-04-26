using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MiniHR.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login");
            }
            return Page();
        }
    }
}
