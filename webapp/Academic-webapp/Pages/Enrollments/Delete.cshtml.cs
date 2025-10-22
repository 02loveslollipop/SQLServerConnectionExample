using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Enrollments;

public class DeleteModel : PageModel
{
    private readonly AcademicContext _context;

    public DeleteModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Enrollment Enrollment { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
                .ThenInclude(c => c!.Instructor)
            .FirstOrDefaultAsync(e => e.EnrollmentID == id);

        if (enrollment is null)
        {
            return NotFound();
        }

        Enrollment = enrollment;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.EnrollmentID == id);
        if (enrollment is null)
        {
            return NotFound();
        }

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
    StatusMessage = "Inscripcion eliminada correctamente.";

        return RedirectToPage("Index");
    }
}
