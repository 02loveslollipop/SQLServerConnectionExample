using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Enrollments;

public class CreateModel : PageModel
{
    private readonly AcademicContext _context;

    public CreateModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Enrollment Enrollment { get; set; } = new() { EnrollmentDate = DateTime.UtcNow.Date };

    public SelectList StudentOptions { get; private set; } = default!;
    public SelectList CourseOptions { get; private set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await PopulateSelectListsAsync();
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

        _context.Enrollments.Add(Enrollment);
        await _context.SaveChangesAsync();

    StatusMessage = "Inscripcion creada correctamente.";
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
