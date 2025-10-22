using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Enrollments;

public class EditModel : PageModel
{
    private readonly AcademicContext _context;

    public EditModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Enrollment Enrollment { get; set; } = new();

    public SelectList StudentOptions { get; private set; } = default!;
    public SelectList CourseOptions { get; private set; } = default!;

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
            .FirstOrDefaultAsync(e => e.EnrollmentID == id);

        if (enrollment is null)
        {
            return NotFound();
        }

        Enrollment = enrollment;
        await PopulateSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateSelectListsAsync();

        var normalizedDate = Enrollment.EnrollmentDate.Date;

        if (normalizedDate > DateTime.UtcNow.Date)
        {
            ModelState.AddModelError("Enrollment.EnrollmentDate", "La fecha de inscripcion no puede ser futura.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Enrollment.EnrollmentDate = DateTime.SpecifyKind(normalizedDate, DateTimeKind.Utc);

        _context.Attach(Enrollment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            StatusMessage = "Inscripcion actualizada correctamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Enrollments.AnyAsync(e => e.EnrollmentID == Enrollment.EnrollmentID))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToPage("Index");
    }

    private async Task PopulateSelectListsAsync()
    {
        StudentOptions = new SelectList(await _context.Students
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Select(s => new { s.StudentID, Display = s.FirstName + " " + s.LastName })
            .ToListAsync(), "StudentID", "Display");

        CourseOptions = new SelectList(await _context.Courses
            .OrderBy(c => c.CourseName)
            .Select(c => new { c.CourseID, Display = c.CourseName + " (" + c.Instructor!.InstructorName + ")" })
            .ToListAsync(), "CourseID", "Display");
    }
}
