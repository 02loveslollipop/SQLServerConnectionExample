using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Instructors;

public class IndexModel : PageModel
{
    private readonly AcademicContext _context;

    public IndexModel(AcademicContext context)
    {
        _context = context;
    }

    public IList<Instructor> Instructors { get; private set; } = new List<Instructor>();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Instructors = await _context.Instructors
            .AsNoTracking()
            .Include(i => i.Department)
            .OrderBy(i => i.InstructorName)
            .ToListAsync();
    }
}
