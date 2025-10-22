using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Courses;

public class DetailsModel : PageModel
{
    private readonly AcademicContext _context;

    public DetailsModel(AcademicContext context)
    {
        _context = context;
    }

    public Course Course { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .AsNoTracking()
            .Include(c => c.Instructor)
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(c => c.CourseID == id);

        if (course is null)
        {
            return NotFound();
        }

        Course = course;
        return Page();
    }
}
