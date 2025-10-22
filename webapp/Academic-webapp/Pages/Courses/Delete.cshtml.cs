using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Courses;

public class DeleteModel : PageModel
{
    private readonly AcademicContext _context;

    public DeleteModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Course Course { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = null)
    {
        if (id is null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .AsNoTracking()
            .Include(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.CourseID == id);

        if (course is null)
        {
            return NotFound();
        }

        Course = course;
        if (saveChangesError == true)
        {
            ErrorMessage = "No se puede eliminar el curso porque esta referenciado por otros registros.";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseID == id);
        if (course is null)
        {
            return NotFound();
        }

        try
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            StatusMessage = $"Curso '{course.CourseName}' eliminado correctamente.";
            return RedirectToPage("Index");
        }
        catch (DbUpdateException)
        {
            return RedirectToPage(new { id, saveChangesError = true });
        }
    }
}
