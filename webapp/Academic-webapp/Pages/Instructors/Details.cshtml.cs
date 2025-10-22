using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Instructors;

public class DetailsModel : PageModel
{
    private readonly AcademicContext _context;

    public DetailsModel(AcademicContext context)
    {
        _context = context;
    }

    public Instructor Instructor { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors
            .AsNoTracking()
            .Include(i => i.Department)
            .Include(i => i.Courses)
            .FirstOrDefaultAsync(i => i.InstructorID == id);

        if (instructor is null)
        {
            return NotFound();
        }

        Instructor = instructor;
        return Page();
    }
}
