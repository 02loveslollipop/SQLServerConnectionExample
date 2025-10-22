using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Students;

public class DetailsModel : PageModel
{
    private readonly AcademicContext _context;

    public DetailsModel(AcademicContext context)
    {
        _context = context;
    }

    public Student Student { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .AsNoTracking()
            .Include(s => s.Major)
                .ThenInclude(m => m!.Department)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.StudentID == id);

        if (student is null)
        {
            return NotFound();
        }

        Student = student;
        return Page();
    }
}
