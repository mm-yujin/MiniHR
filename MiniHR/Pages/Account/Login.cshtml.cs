using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniHR.Models;
using System.Security.Claims;

namespace MiniHR.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;
        public string? ErrorMessage { get; set; }

        public LoginModel(AppDbContext context) => _context = context;

        public async Task<IActionResult> OnPostAsync(string EmployeeNumber, string Password)
        {
            var user = await _context.Employees
                .FirstOrDefaultAsync(u => u.EmployeeNumber.ToString() == EmployeeNumber && u.Password == Password); //일단은 평문 비교, 추후 암호화

            if (user == null)
            {
                ErrorMessage = "아이디 또는 비밀번호가 틀렸습니다.";
                return Page();
            }
                        
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.EmployeeNumber.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/Index"); // 로그인 성공 시 메인으로
        }
    }
}