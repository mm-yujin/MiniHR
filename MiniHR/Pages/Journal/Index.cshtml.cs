using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniHR.Models;

namespace MiniHR.Pages.Journal
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        public IndexModel(AppDbContext context) => _context = context;

        public IList<JournalEntry> JournalEntries { get; set; } = default!;

        public async Task OnGetAsync()
        {
            JournalEntries = await _context.JournalEntries
                .Include(j => j.Details)
                .OrderByDescending(j => j.TransactionDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}