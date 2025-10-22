using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Majors;

public class IndexModel : PageModel
{
    private readonly AcademicContext _context;

    public IndexModel(AcademicContext context)
    {
        _context = context;
    }

    public IList<Major> Majors { get; private set; } = new List<Major>();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Majors = await _context.Majors
            .AsNoTracking()
            .Include(m => m.Department)
            .OrderBy(m => m.MajorName)
            .ToListAsync();
    }
}
