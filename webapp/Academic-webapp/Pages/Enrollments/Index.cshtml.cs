using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Enrollments;

public class IndexModel : PageModel
{
    private readonly AcademicContext _context;

    public IndexModel(AcademicContext context)
    {
        _context = context;
    }

    public IList<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Enrollments = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
                .ThenInclude(c => c!.Instructor)
            .OrderByDescending(e => e.EnrollmentDate)
            .ToListAsync();
    }
}
