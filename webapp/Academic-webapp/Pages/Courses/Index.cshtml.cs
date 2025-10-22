using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Courses;

public class IndexModel : PageModel
{
    private readonly AcademicContext _context;

    public IndexModel(AcademicContext context)
    {
        _context = context;
    }

    public IList<Course> Courses { get; private set; } = new List<Course>();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Courses = await _context.Courses
            .AsNoTracking()
            .Include(c => c.Instructor)
            .OrderBy(c => c.CourseName)
            .ToListAsync();
    }
}
